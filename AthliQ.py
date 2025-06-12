#!/usr/bin/env python
# coding: utf-8

# In[1]:


pip install ultralytics flask opencv-python-headless


# In[2]:


from flask import Flask, request, jsonify
from ultralytics import YOLO
import cv2
import numpy as np
import os

app = Flask(__name__)


# In[3]:


# Load model once
model = YOLO("yolov8n-pose.pt")


# In[4]:


def check_limb_symmetry(keypoints, threshold=20):
    """
    Detects limb asymmetry in a front view image.
    Returns a dictionary with has_problem flag.
    """

    LEFT = [5, 7, 9, 11, 13, 15]   # shoulder, elbow, wrist, hip, knee, ankle
    RIGHT = [6, 8, 10, 12, 14, 16]

    try:
        max_diff = 0
        for l_idx, r_idx in zip(LEFT, RIGHT):
            left_y = keypoints[l_idx][1]
            right_y = keypoints[r_idx][1]
            diff = abs(left_y - right_y)
            max_diff = max(max_diff, diff)

            if diff > threshold:
                return {
                    "max_difference_px": max_diff,
                    "has_problem": True  # Asymmetry detected
                }

        return {
            #"max_difference_px": max_diff,
            "has_problem": False  # Symmetry is OK
        }

    except Exception as e:
        return {
            "error": str(e),
            "has_problem": True  # Treat errors as a problem
        }


# In[5]:


def check_kyphosis(keypoints):
    """
    Detects kyphosis from a side-view image using keypoints (shoulders, hips, head).

    Customizes the kyphosis threshold based on proportions — no ML required.
    """
    import math

    try:
        # Step 1: Extract key anatomical points
        shoulder = (
            (keypoints[5][0] + keypoints[6][0]) / 2,
            (keypoints[5][1] + keypoints[6][1]) / 2,
        )
        hip = (
            (keypoints[11][0] + keypoints[12][0]) / 2,
            (keypoints[11][1] + keypoints[12][1]) / 2,
        )
        head = keypoints[0]

        # Step 2: Define vectors
        vec_back = (shoulder[0] - hip[0], shoulder[1] - hip[1])      # spine
        vec_neck = (head[0] - shoulder[0], head[1] - shoulder[1])    # head angle

        # Step 3: Compute angle between spine and head
        dot = vec_back[0]*vec_neck[0] + vec_back[1]*vec_neck[1]
        norm_back = math.hypot(*vec_back)
        norm_neck = math.hypot(*vec_neck)
        cos_angle = dot / (norm_back * norm_neck + 1e-6)
        angle_rad = math.acos(min(1.0, max(-1.0, cos_angle)))
        acute_angle_deg = math.degrees(angle_rad)
        back_angle_deg = 180 - acute_angle_deg  # open body angle

        # Step 4: Ratio-based custom threshold
        neck_to_back_ratio = norm_neck / (norm_back + 1e-6)
        # Empirical adjustment: flatter backs need lower threshold
        # Values tuned from example data
        angle_threshold = 135 + (10 * (1 - min(neck_to_back_ratio, 1)))

        has_problem = back_angle_deg < angle_threshold

        return {
            #"back_angle_deg": round(back_angle_deg, 2),
            "has_problem": has_problem,
            #"alignment_status": "Possible kyphosis" if has_problem else "OK",
            #"angle_threshold": round(angle_threshold, 2),  # for debug
            #"neck_to_back_ratio": round(neck_to_back_ratio, 2)  # for analysis
        }

    except Exception as e:
        return {
            "has_problem": True,
            "error": str(e),
            "alignment_status": "Error processing keypoints"
        }


# In[6]:


def analyze_pose(image_path, view='front'):
    img = cv2.imread(image_path)
    results = model(img)

    if len(results[0].keypoints.xy) == 0:
        return {"error": "No person detected in image."}

    keypoints = results[0].keypoints.xy[0].cpu().numpy().tolist()

    if view == 'front':
        return check_limb_symmetry(keypoints)
    elif view == 'side':
        return check_spine_alignment(keypoints)
    else:
        return {"error": "Invalid view: choose 'front' or 'side'"}


# In[7]:


@app.route('/analyze', methods=['POST'])
def analyze_images2():
    try:
        front_file = request.files['front_image']
        side_file = request.files['side_image']

        # Convert uploaded images to OpenCV format
        front_np = np.frombuffer(front_file.read(), np.uint8)
        front_img = cv2.imdecode(front_np, cv2.IMREAD_COLOR)

        side_np = np.frombuffer(side_file.read(), np.uint8)
        side_img = cv2.imdecode(side_np, cv2.IMREAD_COLOR)

        # Run YOLO on both images
        front_results = model(front_img)
        side_results = model(side_img)

        # Extract keypoints
        front_keypoints = front_results[0].keypoints.xy[0].cpu().numpy().tolist()
        side_keypoints = side_results[0].keypoints.xy[0].cpu().numpy().tolist()

        front_analysis = check_limb_symmetry(front_keypoints)
        side_analysis = check_kyphosis(side_keypoints)
          
        
        # Determine final result
        if front_analysis.get("has_problem") or side_analysis.get("has_problem"):
            res = False
        else:
            res = True
            
        return jsonify({
            "status": "success",
            
#             "front_view": front_analysis,
#             "side_view": side_analysis
            "result":res
        })

    except Exception as e:
        return jsonify({
            "status": "error",
            "message": str(e)
        })


# In[8]:


@app.route("/run", methods=["GET"])
def health_check():
    return jsonify({"status": "API is running."})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)


# In[ ]:




