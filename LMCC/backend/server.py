from flask import Flask, request, send_file, jsonify,url_for
from flask_cors import CORS
import requests
import json
import os
from collections import defaultdict

from dotenv import load_dotenv

load_dotenv()

app = Flask(__name__)
CORS(app)
api_path = "http://localhost:5000"

# LMCC -> HMD
# actions = []

geo_samples = {"A":[{"name":"Geo Site A","location":"(G,5)"}],"B":[{"name":"Geo Site B","location":"(I,9)"}],"C":[{"name":"Geo Site C","location":"(K,7)"}],"D":[{"name":"Geo Site D","location":"(M,9)"}],"E":[{"name":"Geo Site E","location":"(L,14)"}],"F":[{"name":"Geo Site F","location":"(P,19)"}],"G":[{"name":"Geo Site G","location":"(P,14)"}]}




# HMD -> LMCC
notifications = [
    # Mock
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
]

# Backend configuration
EVA_NUM = 1
FS_ROOT = "root"
MISSION_FOLDER = "mission"
TASK_TITLES = ["egress", "repair", "geo", "ingress"]

TSS_URL = os.environ.get("TSS_URL")
TEAM_NUM = os.environ.get("TEAM_NUM")

if not os.path.exists(FS_ROOT):
    os.makedirs(FS_ROOT)


@app.route("/get-tasks", methods=["GET"])
def get_tasks():
    tasks = []

    for title in TASK_TITLES:
        with open(f"{MISSION_FOLDER}/{title}.json", "r") as f:
            tasks.append(json.load(f))

    return jsonify(tasks), 200


@app.route("/update-state", methods=["POST"])
def update_state():
    data = request.get_json()
    fpath = os.path.join(MISSION_FOLDER, data["taskName"] + ".json")

    with open(fpath, "r") as f:
        state = json.load(f)

    with open(fpath, "w") as f:
        curr_step = 0
        complete = False
        for ins in state["instructions"]:
            for step in ins["steps"]:
                if curr_step == data["step"]:
                    if complete:
                        step["status"] = "inprogress"
                        f.write(json.dumps(state))
                        return jsonify({"message": "Updated app state"}), 200
                    else:
                        complete = True
                        step["status"] = "complete"
                else:
                    curr_step += 1

        f.write(json.dumps(state))

    if complete:
        return jsonify({"message": "Updated app state"}), 200
    else:
        return jsonify({"message": "Failed to update app state"}), 500


# @app.route("/post-action", methods=["POST"])
# def post_action():
#     global actions
#     data = request.get_json()
#     actions.append(data)
#     return jsonify({"message": "Received action"}), 200


# @app.route("/get-actions", methods=["GET"])
# def get_actions():
#     global actions
#     data = jsonify(actions)
#     actions.clear()
#     return data


# @app.route("/get-state", methods=["GET"])
# def get_state():
#     global state
#     global new_state

#     if new_state:
#         new_state = False
#         return jsonify(state)
#     else:
#         return jsonify({})


@app.route("/post-notification", methods=["POST"])
def post_notification():
    global notifications
    data = request.get_json()
    notifications.append(data)
    return jsonify({"message": "Received notification"}), 200


@app.route("/get-notifications", methods=["GET"])
def get_notifications():
    global notifications
    return jsonify(notifications)


@app.route("/upload", methods=["POST"])
def upload_file():
    if "file" not in request.files:
        return jsonify({"message": "No file uploaded"}), 400

    file = request.files["file"]
    fname = file.filename
    if fname == "":
        return jsonify({"message": "No file selected"}), 400

    # Get the file path from the request parameters
    fpath = request.form.get("file_path", "")

    # Create the directory if it doesn't exist
    dir_path = os.path.dirname(os.path.join(FS_ROOT, fpath))
    os.makedirs(dir_path, exist_ok=True)

    full_fpath = os.path.join(FS_ROOT, fpath)
    file.save(full_fpath)

    return jsonify({"message": "File uploaded successfully"}), 200


@app.route("/files", methods=["GET"])
def list_files():
    # Get the directory from the request parameters
    dir = request.args.get("directory", "")
    dir_path = os.path.join(FS_ROOT, dir)
    if not os.path.exists(dir_path):
        return jsonify({"message": "Directory not found"}), 404

    # Obtains the list of files
    files = os.listdir(dir_path)
    return jsonify({"files": files}), 200


@app.route("/download", methods=["GET"])
def download_file():
    fpath = request.args.get("file_path", "")
    full_fpath = os.path.join(FS_ROOT, fpath)
    if not os.path.exists(full_fpath):
        return jsonify({"message": "File not found"}), 404

    return send_file(full_fpath, as_attachment=True)


@app.route("/delete", methods=["DELETE"])
def delete_file():
    fpath = request.args.get("file_path", "")
    full_fpath = os.path.join(FS_ROOT, fpath)
    if not os.path.exists(full_fpath):
        return jsonify({"message": "File not found"}), 404

    os.remove(full_fpath)
    return jsonify({"message": "File deleted successfully"}), 200

@app.route("post-sample/",methods=["POST"])
def post_sample():
    global geo_samples
    data = request.get_json()
    try:
        print(data)
        geo_samples[data['sample_site']].append(data['rock_info'])
        # rock_info
    except Exception as e:
        return jsonify({"message":"no"},400)

    return jsonify({"message":"yay"},200)
@app.route("/num-samples/",methods=["GET"])
def num_samples():
    global geo_samples
    target_site = request.args.get("sample_site")
    return jsonify({"num_samples":len(geo_samples[target_site])})
@app.route("/get-sample/",methods=["GET"])
def get_sample():
    global geo_samples
    station_num = request.args.get('sample_site')
    rock_id = request.args.get('rock_info')
    return jsonify({"sample": geo_samples[station_num][int(rock_id)]}, 200)


@app.route("/get-station/",methods=["GET"])
def get_station_info():
    global geo_samples
    station_num = request.args.get('station_num')
    return jsonify({"station_info":geo_samples[station_num]},200)



@app.route("/get-tss", methods=["GET"])
def get_tss():
    # global tss
    # return jsonify(tss), 200
    res = requests.get(f"{TSS_URL}/json_data/teams/{TEAM_NUM}/TELEMETRY.json")
    return res.json(), 200


if __name__ == "__main__":
    app.run()
