using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AthliQ.Core;
using AthliQ.Core.DTOs.Child;
using AthliQ.Core.DTOs.Sport;
using AthliQ.Core.DTOs.Test;
using AthliQ.Core.Entities.Models;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using AthliQ.Service.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AthliQ.Service.Services.Children
{
    public class ChildService : IChildService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IReportGenerationService _reportGenerationService;
        private readonly IEmailService _emailService;

        public ChildService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            HttpClient httpClient,
            IConfiguration configuration,
            IReportGenerationService reportGenerationService,
            IEmailService emailService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClient = httpClient;
            _configuration = configuration;
            _reportGenerationService = reportGenerationService;
            _emailService = emailService;
        }

        public async Task<GenericResponse<CreationOfChildReturnDto>> CreateChildAsync(
            string userId,
            CreateChildDto createChildDto
        )
        {
            var genericResponse = new GenericResponse<CreationOfChildReturnDto>();

            // Case 01: If Any of entered Data is Invalid
            if (createChildDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Enter Valid Data";
                return genericResponse;
            }

            // Validate Gender
            if (
                createChildDto.Gender.ToLower() != "male"
                && createChildDto.Gender.ToLower() != "female"
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Gender";
                return genericResponse;
            }

            // Validate Date of Birth (Age between 6 and 9 years)
            var currentDate = DateTime.Now;
            var age = currentDate.Year - createChildDto.DateOfBirth.Year;

            // If the current date is before the child's birthday this year, subtract 1 from the age
            if (
                currentDate.Month < createChildDto.DateOfBirth.Month
                || (
                    currentDate.Month == createChildDto.DateOfBirth.Month
                    && currentDate.Day < createChildDto.DateOfBirth.Day
                )
            )
            {
                age--;
            }

            if (age < 6 || age > 9)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Child's age must be between 6 and 9 years.";
                return genericResponse;
            }

            var sportHistory = await _unitOfWork
                .Repository<Sport, int>()
                .GetAsync(createChildDto.SportHistoryId);
            if (sportHistory is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid sport History";

                return genericResponse;
            }

            var parentSportHistory = await _unitOfWork
                .Repository<Sport, int>()
                .GetAsync(createChildDto.ParentSportHistoryId);
            if (parentSportHistory is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid parent sport History";

                return genericResponse;
            }

            var sportPerference = await _unitOfWork
                .Repository<Sport, int>()
                .GetAsync(createChildDto.SportPreferenceId);
            if (sportPerference is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid sport Preference";

                return genericResponse;
            }

            //Upload Photo

            createChildDto.FrontImageName = await DocumentSettings.UploadFile(
                createChildDto.FrontImage,
                "Images"
            );
            createChildDto.SideImageName = await DocumentSettings.UploadFile(
                createChildDto.SideImage,
                "Images"
            );


            var pythonImagecontent = await SendImagesToPythonApiAsync(createChildDto.FrontImage , createChildDto.SideImage);

            if(pythonImagecontent is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Can Not get data from the Python Model";
                return genericResponse;
            }

            var imageIntegratedResult = JsonSerializer.Deserialize<PythonChildImageResultDto>(pythonImagecontent , new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

            if(imageIntegratedResult is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Failed to Deserialize the returned content";
                return genericResponse;
            }

            if(imageIntegratedResult.Status != "success")
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = imageIntegratedResult.Status;
                return genericResponse;
            }

            //

			foreach (var testchild in createChildDto.CreateChildTestDtos)
            {
                var test = await _unitOfWork.Repository<Test, int>().GetAsync(testchild.TestId);
                if (test is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Test You enter";

                    return genericResponse;
                }
            }

            if (
                createChildDto.CreateChildTestDtos[0].TestResult < 65
                || createChildDto.CreateChildTestDtos[0].TestResult > 140
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Standing Long Jump Test";
                return genericResponse;
            }

            if (
                createChildDto.CreateChildTestDtos[1].TestResult < 0.0
                || createChildDto.CreateChildTestDtos[1].TestResult > 18.0
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Sit-and-Reach Flexibility Test";
                return genericResponse;
            }

            if (
                createChildDto.CreateChildTestDtos[2].TestResult < 2.49
                || createChildDto.CreateChildTestDtos[2].TestResult > 10.0
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for One-Leg Stand Test";
                return genericResponse;
            }

            if (
                createChildDto.CreateChildTestDtos[3].TestResult < 5.48
                || createChildDto.CreateChildTestDtos[3].TestResult > 22.0
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Sit-up Test";
                return genericResponse;
            }

            if (
                createChildDto.CreateChildTestDtos[4].TestResult < 125
                || createChildDto.CreateChildTestDtos[4].TestResult > 335
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message =
                    "Out Of Range Input for Medicine Ball Push (1 kg) from Standing Position Test";
                return genericResponse;
            }

            if (
                createChildDto.CreateChildTestDtos[5].TestResult < 3
                || createChildDto.CreateChildTestDtos[5].TestResult > 9
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message =
                    "Out Of Range Input for Straight-Line Walking (3 meters) Test";
                return genericResponse;
            }

            if (
                createChildDto.CreateChildTestDtos[6].TestResult < 5.485
                || createChildDto.CreateChildTestDtos[6].TestResult > 10.295
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for 30-Meter Sprint Test";
                return genericResponse;
            }

            if (
                createChildDto.CreateChildTestDtos[7].TestResult < 5.1
                || createChildDto.CreateChildTestDtos[7].TestResult > 9.6
            )
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for 15-Meter Zigzag Run Test";
                return genericResponse;
            }

            var mappedChild = _mapper.Map<Child>(createChildDto);
            mappedChild.ImageFrontURL = createChildDto.FrontImageName;
            mappedChild.ImageSideURL = createChildDto.SideImageName;
            mappedChild.IsNormalBodyImage = imageIntegratedResult.Result;
            mappedChild.AthliQUserId = userId;
            await _unitOfWork.Repository<Child, int>().AddAsync(mappedChild);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                foreach (var testChild in createChildDto.CreateChildTestDtos)
                {
                    var ChildTest = new ChildTest()
                    {
                        TestId = testChild.TestId,
                        ChildId = mappedChild.Id,
                        TestResult = testChild.TestResult,
                    };

                    await _unitOfWork.Repository<ChildTest, int>().AddAsync(ChildTest);
                }
                var resultCreationOfChildTest = await _unitOfWork.CompleteAsync();
                if (resultCreationOfChildTest > 0)
                {
                    var creationOfChildToReturnDto = new CreationOfChildReturnDto()
                    {
                        ChildId = mappedChild.Id,
                        IsCreated = true,
                    };
                    genericResponse.StatusCode = StatusCodes.Status201Created;
                    genericResponse.Message = "Child and its test Results Created Succesfully";
                    genericResponse.Data = creationOfChildToReturnDto;

                    return genericResponse;
                }
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Create Child and its test Results";
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> DeleteChildAsync(int childId, string userId)
        {
            var genericResponse = new GenericResponse<bool>();
            var child = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.Id == childId && c.AthliQUserId == userId)
                .Result.FirstOrDefaultAsync();
            if (child is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Child Id to delete";

                genericResponse.Data = false;

                return genericResponse;
            }

            child.IsDeleted = true;
            _unitOfWork.Repository<Child, int>().Update(child);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Child Deleted Succesfully";
                genericResponse.Data = true;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Delete Child";
            genericResponse.Data = false;

            return genericResponse;
        }

        public async Task<GenericResponse<ReturnedEvaluateChildDto>> EvaluateDataAsync(int childId)
        {
            var genericResponse = new GenericResponse<ReturnedEvaluateChildDto>();
            var child = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.Id == childId && c.IsDeleted != true)
                .Result.Include(c => c.ChildTests)
                .Include(c => c.ChildResults)
                .Include(c => c.AthliQUser)
                .FirstOrDefaultAsync();

            if (child is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Cannot Evalute Child Data because it is not exist";

                return genericResponse;
            }

            if (child.ChildResults.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This Child is already Evaluated";
                return genericResponse;
            }

            if(child.IsNormalBodyImage == false)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Child has a Problem either in his/her Front or Side Body Posture";
                return genericResponse;
			}

            var preferedSports = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.SportPreferenceId)
                .Result.Include(s => s.Category)
                .FirstOrDefaultAsync();

            var listOfPerferedCategory = new List<string>() { preferedSports.Category.Name };

            var parentSportsHistory = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.ParentSportHistoryId)
                .Result.Include(s => s.Category)
                .FirstOrDefaultAsync();

            var listOfParentCategory = new List<string> { parentSportsHistory.Category.Name };

            var listOfScores = child
                .ChildTests.OrderBy(ct => ct.TestId)
                .Select(ct => ct.TestResult)
                .ToList();

            var ChildTosendDto = new ChildToSendDto()
            {
                Name = child.Name,
                Gender = child.Gender,
                BirthDate = child.DateOfBirth,
                Height = child.Height,
                Weight = child.Weight,
                HasDoctorApproval = child.IsAgreeDoctorApproval,
                HasNormalBloodTest = child.IsNormalBloodTest,
                SchoolType = child.SchoolName,
                PreferredSports = listOfPerferedCategory,
                ParentSportsHistory = listOfParentCategory,
                TestScores = listOfScores,
            };

            var ChildResult = await SendPlayerDataAsync(ChildTosendDto);
            if (ChildResult != null)
            {
                var jsonPolicy = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };
                var integrationResult = JsonSerializer.Deserialize<AllDataChildJavaDto>(
                    ChildResult,
                    jsonPolicy
                );

                if (integrationResult is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Error in integrating with AI Evaluator";

                    return genericResponse;
                }

                if (integrationResult.Errors is not null)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"{integrationResult.Errors.First()}";

                    return genericResponse;
                }

                var result = integrationResult
                    .CategoryScores.Select(kvp => new ChildResultIntegratedDto
                    {
                        Category = kvp.Key,
                        CategoryAr =
                            _unitOfWork
                                .Repository<Category, int>()
                                .Get(c => c.Name == kvp.Key)
                                .Result.Select(c => c.ArabicName)
                                .FirstOrDefault() ?? kvp.Key,
                        Score = kvp.Value,
                    })
                    .ToList();

                var resultWithPercentage = integrationResult
                    .CategoryPercentages.Select(rp => new ChildResultWithPercentagesDto
                    {
                        Category = rp.Key,
                        CategoryAr =
                            _unitOfWork
                                .Repository<Category, int>()
                                .Get(c => c.Name == rp.Key)
                                .Result.Select(c => c.ArabicName)
                                .FirstOrDefault() ?? rp.Key,
                        Percentage = rp.Value,
                    })
                    .ToList();

                var ResultCategoryOfTheChild = result.OrderByDescending(c => c.Score).ElementAt(0);
                var childResultCategory = new ChildResult()
                {
                    ChildId = child.Id,
                    CategoryId = await _unitOfWork
                        .Repository<Category, int>()
                        .Get(c => c.Name == ResultCategoryOfTheChild.Category)
                        .Result.Select(c => c.Id)
                        .FirstOrDefaultAsync(),
                    ResultDate = DateTime.Now,
                };

                await _unitOfWork.Repository<ChildResult, int>().AddAsync(childResultCategory);
                var resultOfCreationChildResult = await _unitOfWork.CompleteAsync();
                if (resultOfCreationChildResult > 0)
                {
                    var sports = await _unitOfWork
                        .Repository<Sport, int>()
                        .Get(s => s.CategoryId == childResultCategory.CategoryId)
                        .Result.ToListAsync();
                    var returnedEvaluatedData = new ReturnedEvaluateChildDto
                    {
                        ChildResultIntegratedDto = result,
                        FinalResult =
                            $"{child.Name}'s Best Category is {integrationResult.BestCategory}",
                        ChildResultWithPercentagesDtos = resultWithPercentage,
                        MatchedSports = _mapper.Map<List<ResultedSportDto>>(sports),
                    };
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Retreived Result succesfully";
                    genericResponse.Data = returnedEvaluatedData;

                    var pdf = await _reportGenerationService.GeneratePdfReportAsync(
                        returnedEvaluatedData,
                        child.Name
                    );
                    var chart = await _reportGenerationService.GenerateChartImageAsync(
                        returnedEvaluatedData
                    );
                    await _emailService.SendReportEmailAsync(
                        child.AthliQUser.Email,
                        child.Name,
                        pdf,
                        chart
                    );
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Failed to Add Result Of Child";

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to retreive Result Of Child";

            return genericResponse;
        }

        public async Task<GenericResponse<ChildTestsGrades>> GetChildTestGradesAsync(int childId)
        {
            var genericResponse = new GenericResponse<ChildTestsGrades>();

            var child = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.Id == childId)
                .Result.Include(c => c.ChildTests)
                .FirstOrDefaultAsync();
            if (child is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Child to Get his/her Test Grades";

                return genericResponse;
            }

			if (child.IsNormalBodyImage == false)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Child has a Problem either in his/her Front or Side Body Posture";
				return genericResponse;
			}


			if (!child.ChildTests.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Child doesn't have test values to get his/her Grades";

                return genericResponse;
            }

            var listOfScores = child
                .ChildTests.OrderBy(ct => ct.TestId)
                .Select(ct => ct.TestResult)
                .ToList();

            var childToSendDto = new ChildToSendWithOnlyScoresDto
            {
                Name = child.Name,
                TestScores = listOfScores,
            };

            var ChildResult = await SendPlayerDataToGetGradesAsync(childToSendDto);
            if (ChildResult is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Failed to Get Child Test Grades";
                return genericResponse;
            }
            var jsonPolicy = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var integrationResult = JsonSerializer.Deserialize<List<JavaChildTestGradesDto>>(
                ChildResult,
                jsonPolicy
            );

            if (integrationResult is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Failed to Deserilaize the returned content";

                return genericResponse;
            }

            var result = integrationResult
                .Select(javaTest => new TestGradesDto
                {
                    TestNameEn = javaTest.TestNameEn,
                    TestNameAr = javaTest.TestNameAr,
                    TestValue = javaTest.TestValue,
                    GradeLevelEn = javaTest.GradeLevel,
                    GradeLevelAr = javaTest.GradeLevelAr,
                })
                .ToList();

            for (int i = 0; i < result.Count; i++)
            {
                result[i].TestId = i + 1;

                if (result[i].TestId == 1 && result[i].GradeLevelEn == "Weak") //Standing Long Jump Test (in cm) (Muscular Strength)
                {
                    result[i].HowToEnhance =
                        "1.Squat Jumps\n2.Broad Jumps\n3.Box Jumps (using a soft box or step)\n4.Lateral Bounds";
                    result[i].HowToEnhanceAr =
                        "1.قفزات القرفصاء\n2.القفزات الأفقية (قفزات العرض)\n3.القفز على الصندوق (باستخدام صندوق ناعم أو درجة)\n4.القفزات الجانبية";
                }
                else if (result[i].TestId == 2 && result[i].GradeLevelEn == "Weak") //Sit-and-Reach Flexibility (in cm) (Muscular Endurance)
                {
                    result[i].HowToEnhance =
                        "1.Seated Forward Bend\n2.Butterfly Stretch\n3.Child's Pose\n4.Standing Forward Bend";
                    result[i].HowToEnhanceAr =
                        "1.إطالة الجلوس للأمام\n2.تمرين الفراشة\n3.وضع الطفل\n4.إطالة الوقوف للأمام";
                }
                else if (result[i].TestId == 3 && result[i].GradeLevelEn == "Weak") //One-Leg Stand (30 seconds) (Balance)
                {
                    result[i].HowToEnhance =
                        "1.One-Leg Balance Hold\n2.Toe Touch While Balancing\n3.Walking on a Line or Beam";
                    result[i].HowToEnhanceAr =
                        "1.الوقوف على ساق واحدة مع الثبات\n2.لمس الأرض أثناء التوازن على ساق واحدة\n3.المشي على خط مستقيم أو عارضة توازن";
                }
                else if (result[i].TestId == 4 && result[i].GradeLevelEn == "Weak") //Sit-up Test (30 seconds) (Muscular Endurance)
                {
                    result[i].HowToEnhance =
                        "1.Crunches\n2.Leg Raises\n3.Bicycle Crunches\n4.Plank";
                    result[i].HowToEnhanceAr =
                        "1.تمرين التقلصات\n2.رفع الساقين\n3.تمرين الدراجة\n4.تمرين اللوح";
                }
                else if (result[i].TestId == 5 && result[i].GradeLevelEn == "Weak") //Medicine Ball Push (1 kg) from Standing Position (Muscular Strength)
                {
                    result[i].HowToEnhance =
                        "1.Overhead Medicine Ball Throw\n2.Chest Pass with Medicine Ball\n3.Wall Slams with Medicine Ball\n4.Power Skips";
                    result[i].HowToEnhanceAr =
                        "1.رمي الكرة الطبية فوق الرأس\n2.دفع الكرة من الصدر\n3.ضرب الكرة بالحائط\n4.التخطي القوي للأعلى";
                }
                else if (result[i].TestId == 6 && result[i].GradeLevelEn == "Weak") //Straight-Line Walking (3 meters) (Balance)
                {
                    result[i].HowToEnhance =
                        "It could be there's a possibility of middle ear issues affecting balance\n1.Heel-to-Toe Walking\n2.Balance on One Leg\n3.Tightrope Walking with Arms Extended\n4.Side Steps on a Line";
                    result[i].HowToEnhanceAr =
                        "يمكن ان تكون هناك مشاكل في الأذن الوسطى تؤثر على التوازن\n1.المشي بالكعب إلى الأصابع\n2.الوقوف على ساق واحدة\n3.المشي على \"حبل مشدود\" وهمي مع تمديد الذراعين\n4.خطوات جانبية على خط مستقيم";
                }
                else if (result[i].TestId == 7 && result[i].GradeLevelEn == "Weak") //30-Meter Sprint (in seconds) (Speed and Agility)
                {
                    result[i].HowToEnhance =
                        "These exercises take into account that speed is largely influenced by genetics but can be enhanced with agility and coordination training\n1.High Knees\n2.Short Sprints with Acceleration\n3.Agility Ladder Drills\n4.Resisted Running with Parachute or Bands";
                    result[i].HowToEnhanceAr =
                        "تأخذ هذه التمارين في الاعتبار أن السرعة تتأثر إلى حد كبير بالجينات ولكن يمكن تعزيزها من خلال تدريب المرونة والتنسيق\n1.رفع الركبتين عاليًا\n2.عدو قصير مع تسارع\n3.تمارين الرشاقة على السلم الأرضي\n4.العدو بالمقاومة باستخدام مظلة أو حبال";
                }
                else if (result[i].TestId == 8 && result[i].GradeLevelEn == "Weak") //15-Meter Zigzag Run (in seconds) (Speed and Agility)
                {
                    result[i].HowToEnhance =
                        "1.Cone Zigzag Drills\n2.Side Shuffles with Quick Turns\n3.T-Drill Agility Test\n4.Figure-8 Running Drill";
                    result[i].HowToEnhanceAr =
                        "1.العدو بين الأقماع على شكل متعرج\n2.الخطوات الجانبية السريعة مع تغيير الاتجاه\n3.اختبار الرشاقة على شكل حرف T\n4.الركض على شكل الرقم 8 بين نقطتين";
                }
                else
                {
                    result[i].HowToEnhance = null;
                    result[i].HowToEnhanceAr = null;
                }
            }

            var childToReturn = new ChildTestsGrades { Name = child.Name, TestGradesDtos = result };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to Retreieve Test Child Grades";
            genericResponse.Data = childToReturn;
            return genericResponse;
        }

        public async Task<GenericResponse<GetAllChildWithTotalCountDto>> ViewAllChildrenAsync(
            string userId,
            string? search,
            int? pageSize = 5,
            int? pageIndex = 1
        )
        {
            var genericResponse = new GenericResponse<GetAllChildWithTotalCountDto>();
            List<GetAllChildDto> getAllChildDtos = new List<GetAllChildDto>();
            var totalCount = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.AthliQUserId == userId && c.IsDeleted != true)
                .Result.CountAsync();
            if (search is not null)
            {
                var SearchedChildren = await _unitOfWork
                    .Repository<Child, int>()
                    .Get(c =>
                        c.AthliQUserId == userId
                        && c.Name.ToLower().Contains(search.ToLower())
                        && c.IsDeleted != true
                    )
                    .Result.ToListAsync();

                if (SearchedChildren.Count == 0)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Children with this name";

                    return genericResponse;
                }

                var mappedFilteredChildren = _mapper.Map<List<GetAllChildDto>>(SearchedChildren);

                foreach (var child in mappedFilteredChildren)
                {
                    var childCategory = await _unitOfWork
                        .Repository<ChildResult, int>()
                        .Get(cr => cr.ChildId == child.Id)
                        .Result.FirstOrDefaultAsync();
                    if (childCategory is null)
                        child.Category = null;
                    else
                    {
                        var category = await _unitOfWork
                            .Repository<Category, int>()
                            .GetAsync(childCategory.CategoryId);
                        if (category is null)
                        {
                            genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                            genericResponse.Message = "Invalid Category As Result";
                            return genericResponse;
                        }
                        child.Category = category.Name;
                        child.CategoryAr = category.ArabicName;

                        getAllChildDtos.Add(child);
                    }
                }
                var returnedSearchedData = new GetAllChildWithTotalCountDto()
                {
                    TotalCount = totalCount,
                    Children = getAllChildDtos,
                };

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Successfully To Search on Children";
                genericResponse.Data = returnedSearchedData;

                return genericResponse;
            }

            var allChildrenOfUser = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.AthliQUserId == userId && c.IsDeleted != true)
                .Result.Skip((pageIndex.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            if (allChildrenOfUser.Count == 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "There are No Children to show";

                return genericResponse;
            }

            var mappedChildren = _mapper.Map<List<GetAllChildDto>>(allChildrenOfUser);

            foreach (var child in mappedChildren)
            {
                var childCategory = await _unitOfWork
                    .Repository<ChildResult, int>()
                    .Get(cr => cr.ChildId == child.Id)
                    .Result.FirstOrDefaultAsync();
                if (childCategory is null)
                    child.Category = null;
                else
                {
                    var category = await _unitOfWork
                        .Repository<Category, int>()
                        .GetAsync(childCategory.CategoryId);
                    if (category is null)
                    {
                        genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                        genericResponse.Message = "Invalid Category As Result";
                        return genericResponse;
                    }
                    child.Category = category.Name;
                    child.CategoryAr = category.ArabicName;

                    getAllChildDtos.Add(child);
                }
            }

            var returnedData = new GetAllChildWithTotalCountDto()
            {
                TotalCount = totalCount,
                Children = getAllChildDtos,
            };
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to retreive all children";
            genericResponse.Data = returnedData;
            return genericResponse;
        }

        public async Task<GenericResponse<GetChildDetailsDto>> ViewChildAsync(int childId)
        {
            var genericResponse = new GenericResponse<GetChildDetailsDto>();

            //Find the Child with the passed Id in the DB with his tests and Resulted Category
            var child = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.Id == childId && c.IsDeleted == false)
                .Result.Include(c => c.ChildTests)
                .Include(c => c.ChildResults)
                .FirstOrDefaultAsync();

            //Check if the child is Null (Not Found)
            if (child is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Child Is Not Found";
                return genericResponse;
            }

            //If Not Null (Found),

            //Find the Prefered Sport and Parent Sport in DB
            var preferedSport = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.SportPreferenceId)
                .Result.Select(s => s.Name)
                .FirstOrDefaultAsync();
            if (preferedSport is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Prefered Sport is Found";
                return genericResponse;
            }

            var preferedSportAr = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.SportPreferenceId)
                .Result.Select(s => s.ArabicName)
                .FirstOrDefaultAsync();
            if (preferedSportAr is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Prefered Sport (arabic) is Found";
                return genericResponse;
            }

            var parentSportHistory = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.ParentSportHistoryId)
                .Result.Select(s => s.Name)
                .FirstOrDefaultAsync();
            if (parentSportHistory is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Sport For The Parent is Found";
                return genericResponse;
            }

            var parentSportHistoryAr = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.ParentSportHistoryId)
                .Result.Select(s => s.ArabicName)
                .FirstOrDefaultAsync();
            if (parentSportHistoryAr is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Sport (arabic) For The Parent is Found";
                return genericResponse;
            }

            //Find Child Tests in DB
            var childTests = await _unitOfWork
                .Repository<ChildTest, int>()
                .Get(ct => ct.ChildId == child.Id)
                .Result.ToListAsync();

            var testWithValueList = new List<TestWithValueDto>();

            if (childTests?.Count > 0)
            {
                foreach (var childTest in childTests)
                {
                    var test = await _unitOfWork
                        .Repository<Test, int>()
                        .Get(t => t.Id == childTest.TestId)
                        .Result.FirstOrDefaultAsync();
                    if (test is null)
                    {
                        genericResponse.StatusCode = StatusCodes.Status200OK;
                        genericResponse.Message = "No Test Values Found";
                        return genericResponse;
                    }

                    testWithValueList.Add(
                        new TestWithValueDto()
                        {
                            Name = test.Name,
                            NameAr = test.ArabicName,
                            TestResult = childTest.TestResult,
                        }
                    );
                }
            }

            //Find Child Result in DB
            var childResult = await _unitOfWork
                .Repository<ChildResult, int>()
                .Get(cr => cr.ChildId == child.Id)
                .Result.FirstOrDefaultAsync();
            if (childResult is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Child Is Not Evaluated To Have A Result";
                return genericResponse;
            }

            var category = await _unitOfWork
                .Repository<Category, int>()
                .Get(c => c.Id == childResult.CategoryId)
                .Result.Select(c => c.Name)
                .FirstOrDefaultAsync();

            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Category Is Found";
                return genericResponse;
            }

            var categoryAr = await _unitOfWork
                .Repository<Category, int>()
                .Get(c => c.Id == childResult.CategoryId)
                .Result.Select(c => c.ArabicName)
                .FirstOrDefaultAsync();

            if (categoryAr is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Category Is Found";
                return genericResponse;
            }

            var sports = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.CategoryId == childResult.CategoryId)
                .Result.Select(s => s.Name)
                .ToListAsync();
            if (!sports.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Sport Is Found";
                return genericResponse;
            }

            var sportsAr = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.CategoryId == childResult.CategoryId)
                .Result.Select(s => s.ArabicName)
                .ToListAsync();
            if (!sportsAr.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Sport Is Found";
                return genericResponse;
            }

            var returnedChild = _mapper.Map<Child, GetChildDetailsDto>(child);
            returnedChild.PreferredSport = preferedSport;
            returnedChild.PreferredSportAr = preferedSportAr;
            returnedChild.ParentSportHistory = parentSportHistory;
            returnedChild.ParentSportHistoryAr = parentSportHistoryAr;
            returnedChild.Tests = testWithValueList;
            returnedChild.Category = category;
            returnedChild.CategoryAr = categoryAr;
            returnedChild.Sports = sports;
            returnedChild.SportsAr = sportsAr;

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Child Is Successfully Retrieved";
            genericResponse.Data = returnedChild;

            return genericResponse;
        }



		private async Task<string> SendImagesToPythonApiAsync(IFormFile frontImage, IFormFile sideImage)
		{
			using var form = new MultipartFormDataContent();

			var frontContent = new StreamContent(frontImage.OpenReadStream());
			frontContent.Headers.ContentType = new MediaTypeHeaderValue(frontImage.ContentType);
			form.Add(frontContent, "front_image", frontImage.FileName);

			var sideContent = new StreamContent(sideImage.OpenReadStream());
			sideContent.Headers.ContentType = new MediaTypeHeaderValue(sideImage.ContentType);
			form.Add(sideContent, "side_image", sideImage.FileName);

			try
			{
				var response = await _httpClient.PostAsync($"{_configuration["Urls:ImageModelUrl"]}analyze", form);
				response.EnsureSuccessStatusCode();

				return await response.Content.ReadAsStringAsync();
			}
			catch (Exception ex)
			{
				return $"Error sending images to Python API: {ex.Message}";
			}
		}



		private async Task<string> SendPlayerDataAsync(object player)
        {
            var jsonOptionsPolicy = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var json = JsonSerializer.Serialize(player, jsonOptionsPolicy);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_configuration["DroolsUrl"]}/player/categorize",
                    content
                );
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $" Error sending data to Drools API: {ex.Message}";
            }
        }

        private async Task<string> SendPlayerDataToGetGradesAsync(object player)
        {
            var jsonOptionsPolicy = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var json = JsonSerializer.Serialize(player, jsonOptionsPolicy);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_configuration["DroolsUrl"]}/player/tests",
                    content
                );
                if (!response.IsSuccessStatusCode)
                {
                    return $"{response.RequestMessage}-{response.Headers}";
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $" Error sending data to Drools API: {ex.Message}";
            }
        }
    }
}
