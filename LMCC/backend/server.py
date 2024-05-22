from flask import Flask, request, send_file, jsonify, url_for
from flask_cors import CORS
import requests
import json
import os
from collections import defaultdict

from dotenv import load_dotenv
count = 2
load_dotenv()

app = Flask(__name__)
CORS(app)
api_path = "http://localhost:5000"

# LMCC -> HMD
# actions = []

geo_samples = {
    "A": [
        {"name": "Geo Site A", "location": "(G,5)"},
        {"rockId": "1","flagged":"true","elements":{
            "SiO2": 36.64,
                "TiO2": 0.92,
                "Al2O3": 8.33,
                "FeO": 18.68,
                "MnO": 0.43,
                "MgO": 6.84,
                "CaO": 5.91,
                "K2O": 0.5,
                "P2O3": 1.19,
                "other": 20.56

        }},
    ],
    "B": [{"name": "Geo Site B", "location": "(I,9)"}],
    "C": [{"name": "Geo Site C", "location": "(J,7)"}],
    "D": [{"name": "Geo Site D", "location": "(L,9)"}],
    "E": [{"name": "Geo Site E", "location": "(K,14)"}],
    "F": [{"name": "Geo Site F", "location": "(O,18)"}],
    "G": [{"name": "Geo Site G", "location": "(O,14)"}],
}

# HMD -> LMCC
notifications = [
    # Mock
    {"message": "Begin EVA", "timestamp": "00:00:00"},
]

# Backend configuration
EVA_NUM = 1
FS_ROOT = "root"
MISSION_FOLDER = "mission"
TASK_TITLES = ["egress", "inspection", "repair", "geo", "ingress"]

# Task status
INCOMPLETE = "incomplete"
INPROGRESS = "inprogress"
COMPLETE = "complete"
end_point = [0,0]

pois = []
tasks = []

rock_id_to_station_idx = {1:1}
for title in TASK_TITLES:
    with open(f"{MISSION_FOLDER}/{title}.json", "r") as f:
        tasks.append(json.load(f))
tasks[0]["instructions"][0]["steps"][0]["status"] = INPROGRESS

TSS_URL = os.environ.get("TSS_URL")
TEAM_NUM = os.environ.get("TEAM_NUM")

if not os.path.exists(FS_ROOT):
    os.makedirs(FS_ROOT)


@app.route("/get-tasks", methods=["GET"])
def get_tasks():
    global tasks
    return jsonify(tasks), 200


@app.route("/update-state", methods=["POST"])
def update_state():
    data = request.get_json()
    curr_task = None
    for task in tasks:
        if task["name"] == data["taskName"]:
            curr_task = task

    if curr_task is None:
        return jsonify({"message": "Task does not exist"}), 500

    curr_step = 0
    complete = False
    for ins in curr_task["instructions"]:
        for step in ins["steps"]:
            if curr_step == data["step"]:
                if complete:
                    step["status"] = INPROGRESS
                    return jsonify({"message": "Updated app state"}), 200
                else:
                    complete = True
                    step["status"] = COMPLETE
            else:
                curr_step += 1

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


@app.route("/update-sample", methods=["POST"])
def post_sample(station_num):
    global geo_samples
    print(request)
    data = request.get_json()
    try:
        print(data)
        geo_samples[data["sample_site"]].append(data["rock_info"])
        # rock_info
    except Exception as e:
        print(e)
        return jsonify({"message": "no"}, 400)

    return jsonify({"message": "yay"}), 200


@app.route("/num-samples/<sample_site>", methods=["GET"])
def num_samples(sample_site):
    global geo_samples

    return jsonify({"num_samples": len(geo_samples[sample_site])})


@app.route("/get-sample", methods=["GET"])
def get_sample():
    global geo_samples
    global rock_id_to_station_idx
    station_num = request.args.get("sample_site")
    rock_id = request.args.get("rock_id")
    return jsonify({"sample": geo_samples[station_num][rock_id_to_station_idx[int(rock_id)]]}), 200


@app.route("/make-rock",methods=['POST'])
def make_rock():
    global count
    global rock_id_to_station_idx
    print(request.get_json())
    data = request.get_json()
    geo_samples[data['station_id']].append(data['rock'])
    geo_samples[data['station_id']][len(geo_samples[data['station_id']]) - 1]['rockId'] = count
    rock_id_to_station_idx[count] = len(geo_samples[data['station_id']]) - 1
    geo_samples[data['station_id']][len(geo_samples[data['station_id']]) - 1]['flagged'] = False
    count += 1
    print(geo_samples[data['station_id']])
    return jsonify({"sample":"gg"}), 200

@app.route("/flag-rock",methods=["POST"])
def update_spec_rock():
    global geo_samples
    global rock_id_to_station_idx

    data = request.get_json()
    geo_samples[data['station_id']][rock_id_to_station_idx[int(data['rock_id'])]]['flagged'] = data['flagged']
    return jsonify({'good':"gg"}),200

@app.route("/get-station", methods=["GET"])
def get_station_info():
    global geo_samples
    station_num = request.args.get("station_num")
    return jsonify({"station_info": geo_samples[station_num][0]}), 200


@app.route("/get-tss", methods=["GET"])
def get_tss():
    # global tss
    # return jsonify(tss), 200
    return jsonify({"x":"y"}),200
    res = requests.get(f"{TSS_URL}/json_data/teams/{TEAM_NUM}/TELEMETRY.json")
    return res.json(), 200


mock_imu = {
    "imu": {
        "eva1": {"posx": 298355, "posy": 3272383, "heading": 0.000000},
        "eva2": {"posx": 298355, "posy": 3272383, "heading": 0.000000},
    }
}


@app.route("/get-imu", methods=["GET"])
def get_imu():
    # res = requests.get(f"{TSS_URL}/json_data/IMU.json")
    # return res.json(), 200
    return jsonify(mock_imu), 200


@app.route("/get-rover", methods=["GET"])
def get_rover():
    res = requests.get(f"{TSS_URL}/json_data/ROVER.json")
    return res.json(), 200

@app.route("/draw-end",methods=["GET"])
def send_draw():
    global end_point
    print(request.args);
    x = request.args.get("x")
    y = request.args.get("y")
    end_point[0] = x
    end_point[1] = y

    return jsonify({'good':"gg"}), 200

@app.route("/send-poi",methods=['get'])
def send_poi():
    global pois
    x = request.args.get("x")
    y = request.args.get("y")
    r = []
    r.append(x)
    r.append(y)
    pois.append(r)
    return jsonify({'good':"gg"}), 200


@app.route("/get-pois",methods=['get'])
def get_poi():
    global pois
    if(pois):
        to_send = {'poi_list':pois}
        pois = []
        return jsonify(to_send),200
    else:
        return jsonify({'poi_list':[]}),200

pois_our = []
@app.route("/lmcc-send-poi",methods=['get'])
def send_poi_l():
        global pois_our
        x = request.args.get("x")
        y = request.args.get("y")
        types = request.args.get("types")
        r = []
        r.append(x)
        r.append(y)
        r.append(types)
        pois.append(r)
        return jsonify({'good': "gg"}), 200

@app.route("/get-pois-l",methods=['get'])
def get_poi_l():
    global pois_our
    if pois_our:
        to_send = {'poi_list':pois_our}
        pois_our = []
        return jsonify(to_send),200
    else:
        return jsonify({'poi_list':[]}),200
if __name__ == "__main__":
    app.run()
