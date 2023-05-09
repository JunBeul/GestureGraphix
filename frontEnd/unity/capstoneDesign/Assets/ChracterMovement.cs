using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalidokit{
    
public class ChracterMovement : MonoBehaviour
{
   
    public static void main(){

var remap = Kalidokit.Utils.remap;
var clamp = Kalidokit.Utils.clamp;
var lerp = Kalidokit.Vector.lerp;
 
/* THREEJS WORLD SETUP */
var currentVrm;

int dampener=0,lerpAmount=0,rotation=0;

// renderer
var renderer = new THREE.WebGLRenderer( alpha: true );
renderer.setSize(window.innerWidth, window.innerHeight);
renderer.setPixelRatio(window.devicePixelRatio);
document.body.appendChild(renderer.domElement); 

// camera
var orbitCamera = new THREE.PerspectiveCamera(35, window.innerWidth / window.innerHeight, 0.1f, 1000);
orbitCamera.position.set(0.0f, 1.4f, 0.7f);

// controls
var orbitControls = new THREE.OrbitControls(orbitCamera, renderer.domElement);
orbitControls.screenSpacePanning=true;
orbitControls.target.set(0.0f, 1.4f, 0.0f);
orbitControls.update();

// scene
var scene = new THREE.Scene();


// Main Render Loop


void animate (){
    requestAnimationFrame(animate);

    if (currentVrm) {
        // Update model to render physics
        currentVrm.update(clock.getDelta());
    }
    renderer.render(scene, orbitCamera);
};
animate();

/* VRM CHARACTER SETUP */


// Animate Rotation Helper function
int rigRotation = (Kalidokit, rotation : ( x : 0, y: 0, z: 0 ), dampener = 1, lerpAmount = 0.3);{
    if (!currentVrm) {
        return;
    }
    var Part = currentVrm.humanoid.getBoneNode(THREE.VRMSchema.HumanoidBoneName[name]);
    if (!Part) {
        return;
    }

    double euler = new THREE.Euler(
        rotation.x * dampener,
        rotation.y * dampener,
        rotation.z * dampener,
        rotation.rotationOrder || "XYZ"
    );
    var quaternion = new THREE.Quaternion().setFromEuler(euler);
    Part.quaternion.slerp(quaternion, lerpAmount); // interpolate
};

// Animate Position Helper Functio
var rigPosition = (Kalidokit, position : ( x: 0, y: 0, z: 0 ), dampener = 1, lerpAmount = 0.3f) ; {
    if (!currentVrm) {
        return;
    }
    var Part = currentVrm.humanoid.getBoneNode(THREE.VRMSchema.HumanoidBoneName[name]);
    if (!Part) {
        return;
    }
    var vector = new THREE.Vector3(position.x * dampener, position.y * dampener, position.z * dampener);
    Part.position.lerp(vector, lerpAmount); // interpolate
};

var oldLookTarget = new THREE.Euler();
var rigFace = (riggedFace) => {
    if (!currentVrm) {
        return;
    }
    rigRotation("Neck", riggedFace.head, 0.7f);

    // Blendshapes and Preset Name Schema
    var Blendshape = currentVrm.blendShapeProxy;
    var PresetName = THREE.VRMSchema.BlendShapePresetName;

    // Simple example without winking. Interpolate based on old blendshape, then stabilize blink with `Kalidokit` helper function.
    // for VRM, 1 is closed, 0 is open.
    riggedFace.eye.l = lerp(clamp(1 - riggedFace.eye.l, 0, 1), Blendshape.getValue(PresetName.Blink), 0.5f);
    riggedFace.eye.r = lerp(clamp(1 - riggedFace.eye.r, 0, 1), Blendshape.getValue(PresetName.Blink), 0.5f);
    riggedFace.eye = Kalidokit.Face.stabilizeBlink(riggedFace.eye, riggedFace.head.y);
    Blendshape.setValue(PresetName.Blink, riggedFace.eye.l);

    // Interpolate and set mouth blendshapes
    Blendshape.setValue(PresetName.I, lerp(riggedFace.mouth.shape.I, Blendshape.getValue(PresetName.I), 0.5f));
    Blendshape.setValue(PresetName.A, lerp(riggedFace.mouth.shape.A, Blendshape.getValue(PresetName.A), 0.5f));
    Blendshape.setValue(PresetName.E, lerp(riggedFace.mouth.shape.E, Blendshape.getValue(PresetName.E), 0.5f));
    Blendshape.setValue(PresetName.O, lerp(riggedFace.mouth.shape.O, Blendshape.getValue(PresetName.O), 0.5f));
    Blendshape.setValue(PresetName.U, lerp(riggedFace.mouth.shape.U, Blendshape.getValue(PresetName.U), 0.5f));

    //PUPILS
    //interpolate pupil and keep a copy of the value
    var lookTarget = new THREE.Euler(
        lerp(oldLookTarget.x, riggedFace.pupil.y, 0.4f),
        lerp(oldLookTarget.y, riggedFace.pupil.x, 0.4f),
        0,
        "XYZ"
    );
    oldLookTarget.copy(lookTarget);
    currentVrm.lookAt.applyer.lookAt(lookTarget);
};

/* VRM Character Animator */
var animateVRM = (vrm, results) => {
    if (!vrm) {
        return;
    }
    // Take the results from `Holistic` and animate character based on its Face, Pose, and Hand Keypoints.
   var riggedPose, riggedLeftHand, riggedRightHand, riggedFace;

    var faceLandmarks = results.faceLandmarks;
    // Pose 3D Landmarks are with respect to Hip distance in meters
    var pose3DLandmarks = results.ea;
    // Pose 2D landmarks are with respect to videoWidth and videoHeight
    var pose2DLandmarks = results.poseLandmarks;
    // Be careful, hand landmarks may be reversed
    var leftHandLandmarks = results.rightHandLandmarks;
    var rightHandLandmarks = results.leftHandLandmarks;

    // Animate Face
    if (faceLandmarks) {
        riggedFace = Kalidokit.Face.solve(faceLandmarks, 
            runtime: "mediapipe",
            video: videoElement
        );
        rigFace(riggedFace);
    }

    // Animate Pose
    if (pose2DLandmarks && pose3DLandmarks) {
        riggedPose = Kalidokit.Pose.solve(pose3DLandmarks, pose2DLandmarks, 
            runtime: "mediapipe",
            video: videoElement
        );
        rigRotation("Hips", riggedPose.Hips.rotation, 0.7f);
        rigPosition(
            "Hips",
            
                x= riggedPose.Hips.position.x, // Reverse direction
                y= riggedPose.Hips.position.y + 1, // Add a bit of height
                z= -riggedPose.Hips.position.z // Reverse direction
            ,
            1,
            0.07f
        );

        rigRotation("Chest", riggedPose.Spine, 0.25f, 0.3f);
        rigRotation("Spine", riggedPose.Spine, 0.45f, 0.3f);

        rigRotation("RightUpperArm", riggedPose.RightUpperArm, 1, 0.3f);
        rigRotation("RightLowerArm", riggedPose.RightLowerArm, 1, 0.3f);
        rigRotation("LeftUpperArm", riggedPose.LeftUpperArm, 1, 0.3f);
        rigRotation("LeftLowerArm", riggedPose.LeftLowerArm, 1, 0.3f);

        rigRotation("LeftUpperLeg", riggedPose.LeftUpperLeg, 1, 0.3f);
        rigRotation("LeftLowerLeg", riggedPose.LeftLowerLeg, 1, 0.3f);
        rigRotation("RightUpperLeg", riggedPose.RightUpperLeg, 1, 0.3f);
        rigRotation("RightLowerLeg", riggedPose.RightLowerLeg, 1, 0.3f);
    }

    // Animate Hands
    if (leftHandLandmarks) {
        riggedLeftHand = Kalidokit.Hand.solve(leftHandLandmarks, "Left");
        rigRotation("LeftHand", 
            // Combine pose rotation Z and hand rotation X Y
            z: riggedPose.LeftHand.z,
            y: riggedLeftHand.LeftWrist.y,
            x: riggedLeftHand.LeftWrist.x
        );
        rigRotation("LeftRingProximal", riggedLeftHand.LeftRingProximal);
        rigRotation("LeftRingIntermediate", riggedLeftHand.LeftRingIntermediate);
        rigRotation("LeftRingDistal", riggedLeftHand.LeftRingDistal);
        rigRotation("LeftIndexProximal", riggedLeftHand.LeftIndexProximal);
        rigRotation("LeftIndexIntermediate", riggedLeftHand.LeftIndexIntermediate);
        rigRotation("LeftIndexDistal", riggedLeftHand.LeftIndexDistal);
        rigRotation("LeftMiddleProximal", riggedLeftHand.LeftMiddleProximal);
        rigRotation("LeftMiddleIntermediate", riggedLeftHand.LeftMiddleIntermediate);
        rigRotation("LeftMiddleDistal", riggedLeftHand.LeftMiddleDistal);
        rigRotation("LeftThumbProximal", riggedLeftHand.LeftThumbProximal);
        rigRotation("LeftThumbIntermediate", riggedLeftHand.LeftThumbIntermediate);
        rigRotation("LeftThumbDistal", riggedLeftHand.LeftThumbDistal);
        rigRotation("LeftLittleProximal", riggedLeftHand.LeftLittleProximal);
        rigRotation("LeftLittleIntermediate", riggedLeftHand.LeftLittleIntermediate);
        rigRotation("LeftLittleDistal", riggedLeftHand.LeftLittleDistal);
    }
    if (rightHandLandmarks) {
        riggedRightHand = Kalidokit.Hand.solve(rightHandLandmarks, "Right");
        rigRotation("RightHand", 
            // Combine Z axis from pose hand and X/Y axis from hand wrist rotation
            z: riggedPose.RightHand.z,
            y: riggedRightHand.RightWrist.y,
            x: riggedRightHand.RightWrist.x
    );
        rigRotation("RightRingProximal", riggedRightHand.RightRingProximal);
        rigRotation("RightRingIntermediate", riggedRightHand.RightRingIntermediate);
        rigRotation("RightRingDistal", riggedRightHand.RightRingDistal);
        rigRotation("RightIndexProximal", riggedRightHand.RightIndexProximal);
        rigRotation("RightIndexIntermediate", riggedRightHand.RightIndexIntermediate);
        rigRotation("RightIndexDistal", riggedRightHand.RightIndexDistal);
        rigRotation("RightMiddleProximal", riggedRightHand.RightMiddleProximal);
        rigRotation("RightMiddleIntermediate", riggedRightHand.RightMiddleIntermediate);
        rigRotation("RightMiddleDistal", riggedRightHand.RightMiddleDistal);
        rigRotation("RightThumbProximal", riggedRightHand.RightThumbProximal);
        rigRotation("RightThumbIntermediate", riggedRightHand.RightThumbIntermediate);
        rigRotation("RightThumbDistal", riggedRightHand.RightThumbDistal);
        rigRotation("RightLittleProximal", riggedRightHand.RightLittleProximal);
        rigRotation("RightLittleIntermediate", riggedRightHand.RightLittleIntermediate);
        rigRotation("RightLittleDistal", riggedRightHand.RightLittleDistal);
    }
};
     

/* SETUP MEDIAPIPE HOLISTIC INSTANCE */
int videoElement = document.querySelector(".input_video"),
    guideCanvas = document.querySelector("canvas.guides");

var onResults = (results) => {
    // Draw landmark guides
    drawResults(results);
    // Animate model
    animateVRM(currentVrm, results);
};

var holistic = new Holistic(
    locateFile: (file) => {
        return "https://cdn.jsdelivr.net/npm/@mediapipe/holistic@0.5f.1635989137/${file}";
    }
);

holistic.setOptions(
    modelComplexity: 1,
    smoothLandmarks: true,
    minDetectionConfidence: 0.7f,
    minTrackingConfidence: 0.7f,
    refineFaceLandmarks: true
);
// Pass holistic a callback function
holistic.onResults(onResults);

var drawResults = (results) => {
    guideCanvas.width = videoElement.videoWidth;
    guideCanvas.height = videoElement.videoHeight;
    let canvasCtx = guideCanvas.getContext("2d");
    canvasCtx.save();
    canvasCtx.clearRect(0, 0, guideCanvas.width, guideCanvas.height);
    // Use `Mediapipe` drawing functions
    drawConnectors(canvasCtx, results.poseLandmarks, POSE_CONNECTIONS, 
        color: "#00cff7",
        lineWidth: 4
    );
    drawLandmarks(canvasCtx, results.poseLandmarks, 
        color: "#ff0364",
        lineWidth: 2
    );
    drawConnectors(canvasCtx, results.faceLandmarks, FACEMESH_TESSELATION, 
        color: "#C0C0C070",
        lineWidth: 1
    );
    if (results.faceLandmarks && results.faceLandmarks.length == 478) {
        //draw pupils
        drawLandmarks(canvasCtx, results.faceLandmarks[468], results.faceLandmarks[468 + 5], 
            color: "#ffe603",
            lineWidth: 2
    );
    }
    drawConnectors(canvasCtx, results.leftHandLandmarks, HAND_CONNECTIONS, 
        color: "#eb1064",
        lineWidth: 5
    );
    drawLandmarks(canvasCtx, results.leftHandLandmarks, 
        color: "#00cff7",
        lineWidth: 2
    );
    drawConnectors(canvasCtx, results.rightHandLandmarks, HAND_CONNECTIONS, 
        color: "#22c3e3",
        lineWidth: 5
    );
    drawLandmarks(canvasCtx, results.rightHandLandmarks, 
        color: "#ff0364",
        lineWidth: 2
    );



// Use `Mediapipe` utils to get camera - lower resolution = higher fps
var camera = new Camera(videoElement, 
    onFrame: async () => {
        await holistic.send( image: videoElement );
    },
    width: 640,
    height: 480
);
camera.start();
            };
        }
   } 
    }
