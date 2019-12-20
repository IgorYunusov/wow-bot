﻿using System;
using System.Runtime.InteropServices;
using IceFlake.Client.Patchables;
#if SLIMDX
using SlimDX;
#else
using IceFlake.DirectX;

#endif

namespace IceFlake.Client
{
    public unsafe class WoWCamera
    {
        public WoWCamera()
        {
            Pointer =
                Manager.Memory.Read<IntPtr>(
                    new IntPtr(Manager.Memory.Read<uint>((IntPtr) Pointers.Drawing.WorldFrame) +
                               Pointers.Drawing.ActiveCamera));
        }

        public IntPtr Pointer { get; private set; }

        public bool IsValid
        {
            get { return Pointer != IntPtr.Zero; }
        }

        // TODO: Implement these
        public Vector3 Forward
        {
            get
            {
                if (_Forward == null)
                    _Forward =
                        Manager.Memory.RegisterDelegate<ForwardDelegate>(Manager.Memory.GetObjectVtableFunction(
                            Pointer, 1));

                var res = new Vector3();
                _Forward(Pointer, &res);
                return res;
            }
        }

        public Vector3 Right
        {
            get
            {
                if (_Right == null)
                    _Right =
                        Manager.Memory.RegisterDelegate<RightDelegate>(Manager.Memory.GetObjectVtableFunction(Pointer, 2));

                var res = new Vector3();
                _Right(Pointer, &res);
                return res;
            }
        }

        public Vector3 Up
        {
            get
            {
                if (_Up == null)
                    _Up = Manager.Memory.RegisterDelegate<UpDelegate>(Manager.Memory.GetObjectVtableFunction(Pointer, 3));

                var res = new Vector3();
                _Up(Pointer, &res);
                return res;
            }
        }

        public Matrix Projection
        {
            get
            {
                CameraInfo cam = GetCamera();
                return Matrix.PerspectiveFovRH(cam.FieldOfView*0.6f, cam.Aspect, cam.NearPlane, cam.FarPlane);
            }
        }

        public Matrix View
        {
            get
            {
                CameraInfo cam = GetCamera();
                Vector3 eye = cam.Position;
                Vector3 at = eye + Forward;
                return Matrix.LookAtRH(eye, at, new Vector3(0, 0, 1));
            }
        }

        #region Typedefs & Delegates

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate Vector3* ForwardDelegate(IntPtr ptr, Vector3* vecOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate Vector3* RightDelegate(IntPtr ptr, Vector3* vecOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate Vector3* UpDelegate(IntPtr ptr, Vector3* vecOut);

        private ForwardDelegate _Forward;
        private RightDelegate _Right;

        private UpDelegate _Up;

        #endregion

        public CameraInfo GetCamera()
        {
            return Manager.Memory.Read<CameraInfo>(Pointer);
        }
    }
}