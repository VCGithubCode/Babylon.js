﻿using System.Collections.Generic;
using Autodesk.Max;
using BabylonExport.Entities;

namespace Max2Babylon
{
    partial class BabylonExporter
    {
        private void ExportCamera(IIGameNode cameraNode, BabylonScene babylonScene)
        {

            if (cameraNode.MaxNode.GetBoolProperty("babylonjs_noexport"))
            {
                return;
            }
            var gameCamera = cameraNode.IGameObject.AsGameCamera();
            var initialized = gameCamera.InitializeData;
            var babylonCamera = new BabylonCamera();

            RaiseMessage(cameraNode.Name, 1);
            babylonCamera.name = cameraNode.Name;
            babylonCamera.id = cameraNode.MaxNode.GetGuid().ToString();
            if (cameraNode.NodeParent != null)
            {
                babylonCamera.parentId = cameraNode.NodeParent.MaxNode.GetGuid().ToString();
            }

            float fov = 0;
            gameCamera.CameraFOV.GetPropertyValue(ref fov, 0, false);
            babylonCamera.fov = fov;
            float minZ = 0;
            gameCamera.CameraNearClip.GetPropertyValue(ref minZ, 0, false);
            babylonCamera.minZ = minZ;
            float maxZ = 0;
            gameCamera.CameraFarClip.GetPropertyValue(ref maxZ, 0, false);
            babylonCamera.maxZ = maxZ;

            if (babylonCamera.minZ == 0.0f)
            {
                babylonCamera.minZ = 0.1f;
            }

            // Control
            babylonCamera.speed = cameraNode.MaxNode.GetFloatProperty("babylonjs_speed", 1.0f);
            babylonCamera.inertia = cameraNode.MaxNode.GetFloatProperty("babylonjs_inertia", 0.9f);

            // Collisions
            babylonCamera.checkCollisions = cameraNode.MaxNode.GetBoolProperty("babylonjs_checkcollisions");
            babylonCamera.applyGravity = cameraNode.MaxNode.GetBoolProperty("babylonjs_applygravity");
            babylonCamera.ellipsoid = cameraNode.MaxNode.GetVector3Property("babylonjs_ellipsoid");

            // Position
            var wm = cameraNode.GetObjectTM(0);
            var position = wm.Translation;
            babylonCamera.position = new float[] { position.X, position.Y, position.Z };

            // Target
            var target = gameCamera.CameraTarget;
            if (target != null)
            {
                babylonCamera.lockedTargetId = target.MaxNode.GetGuid().ToString();
            }
            else
            {
                IPoint3 cameraTargetDist = Loader.Global.Point3.Create();
                gameCamera.CameraTargetDist.GetPropertyValue(cameraTargetDist, 0);
                var targetPos = position.Add(cameraTargetDist);
                babylonCamera.target = new float[] { targetPos.X, targetPos.Y, targetPos.Z };
            }

            // todo : handle animations
            //// Animations
            //var animations = new List<BabylonAnimation>();
            //cameraNode.IGameControl.
            //if (!ExportVector3Controller(cameraNode.TMController.PositionController, "position", animations))
            //{
            //    ExportVector3Animation("position", animations, key =>
            //    {
            //        var worldMatrix = cameraNode.GetWorldMatrix(key, cameraNode.HasParent());
            //        return worldMatrix.Trans.ToArraySwitched();
            //    });
            //}

            //ExportFloatAnimation("fov", animations, key => new[] {Tools.ConvertFov(maxCamera.GetFOV(key, Tools.Forever))});

            //babylonCamera.animations = animations.ToArray();

            //if (cameraNode.GetBoolProperty("babylonjs_autoanimate"))
            //{
            //    babylonCamera.autoAnimate = true;
            //    babylonCamera.autoAnimateFrom = (int)cameraNode.GetFloatProperty("babylonjs_autoanimate_from");
            //    babylonCamera.autoAnimateTo = (int)cameraNode.GetFloatProperty("babylonjs_autoanimate_to");
            //    babylonCamera.autoAnimateLoop = cameraNode.GetBoolProperty("babylonjs_autoanimateloop");
            //}

            babylonScene.CamerasList.Add(babylonCamera);
        }
    }
}
