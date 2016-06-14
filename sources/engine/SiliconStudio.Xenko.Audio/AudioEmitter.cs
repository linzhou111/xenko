﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Native;

namespace SiliconStudio.Xenko.Audio
{
    /// <summary>
    /// Represents a 3D audio emitter in the audio scene. 
    /// This object, used in combination with an <see cref="AudioListener"/>, can simulate 3D audio localization effects for a given sound implementing the <see cref="IPositionableSound"/> interface.
    /// For more details take a look at the <see cref="IPositionableSound.Apply3D"/> function.
    /// </summary>
    /// <seealso cref="IPositionableSound.Apply3D"/>
    /// <seealso cref="AudioListener"/>
    public class AudioEmitter
    {
        /// <summary>
        /// The position of the emitter in the 3D world.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The velocity of the emitter in the 3D world. 
        /// </summary>
        /// <remarks>This is only used to calculate the doppler effect on the sound effect</remarks>
        public Vector3 Velocity;

        private Vector3 up;

        /// <summary>
        /// Gets or sets the Up orientation vector for this listener. This vector up of the world for the listener.
        /// </summary>
        /// <remarks>
        /// <para>By default, this value is (0,1,0).</para>
        /// <para>The value provided will be normalized if it is not already.</para>
        /// <para>The values of the Forward and Up vectors must be orthonormal (at right angles to one another). 
        /// Behavior is undefined if these vectors are not orthonormal.</para>
        /// <para>Doppler and Matrix values between an <see name="AudioEmitter"/> and an <see cref="AudioListener"/> are effected by the listener orientation.</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The value provided to the set accessor is (0,0,0).</exception>
        public Vector3 Up
        {
            get
            {
                return up;
            }
            set
            {
                if (value == Vector3.Zero)
                    throw new InvalidOperationException("The value of the Up vector can not be (0,0,0)");

                up = Vector3.Normalize(value);
            }
        }

        private Vector3 forward;

        /// <summary>
        /// Gets or sets the forward orientation vector for this listener. This vector represents the orientation the listener is looking at.
        /// </summary>
        /// <remarks>
        /// <para>By default, this value is (0,0,1).</para>
        /// <para>The value provided will be normalized if it is not already.</para>
        /// <para>The values of the Forward and Up vectors must be orthonormal (at right angles to one another). 
        /// Behavior is undefined if these vectors are not orthonormal.</para>
        /// <para>Doppler and Matrix values between an <see name="AudioEmitter"/> and an <see cref="AudioListener"/> are effected by the listener orientation.</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The value provided to the set accessor is (0,0,0) or <see cref="Up"/>.</exception>
        public Vector3 Forward
        {
            get
            {
                return forward;
            }
            set
            {
                if (value == Vector3.Zero)
                    throw new InvalidOperationException("The value of the Forward vector can not be (0,0,0)");

                forward = Vector3.Normalize(value);
            }
        }

        public AudioEmitter()
        {
            DopplerScale = 1;
            DistanceScale = 1;
            Forward = new Vector3(0, 0, 1);
            Up = new Vector3(0, 1, 0);
        }

        internal unsafe void Apply3D(AudioListener listener, uint source)
        {
            OpenAl.SourcePush3D(listener.Listener, source, (float*)Interop.Fixed(ref Position), (float*)Interop.Fixed(ref forward), (float*)Interop.Fixed(ref up), (float*)Interop.Fixed(ref Velocity));
        }

        /// <summary>
        /// The scalar applied to the level of Doppler effect calculated between this and the listener
        /// </summary>
        /// <remarks>
        /// By default, this value is 1.0.
        /// This value determines how much to modify the calculated Doppler effect between this object and a AudioListener. 
        /// Values below 1.0 scale down the Doppler effect to make it less apparent. 
        /// Values above 1.0 exaggerate the Doppler effect. A value of 1.0 leaves the effect unmodified.
        /// Note that this value modifies only the calculated Doppler between this object and a AudioListener. 
        /// The calculated Doppler is a product of the relationship between AudioEmitter.Velocity and AudioListener.Velocity. 
        /// If the calculation yields a result of no Doppler effect, this value has no effect.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The doppler scale of an audio emitter must be greater than or equal to zero.</exception>
        public float DopplerScale
        {
            get
            {
                return dopplerScale;
            }

            set 
            { 
                if (value < 0) 
                    throw new ArgumentOutOfRangeException();

                dopplerScale = value;
            }
        }

        private float dopplerScale;

        /// <summary>
        /// Distance scale used to calculate the signal attenuation with the listener
        /// </summary>
        /// <remarks>
        /// By default, this value is 1.0.
        /// This value represent the distance unit and determines how quicly the signal attenuates between this object and the AudioListener. 
        /// Values below 1.0 exaggerate the attenuation to make it more apparent. 
        /// Values above 1.0 scale down the attenuation. A value of 1.0 leaves the default attenuation unchanged.
        /// Note that this value modifies only the calculated attenuation between this object and a AudioListener. 
        /// The calculated attenuation is a product of the relationship between AudioEmitter.Position and AudioListener.Position. 
        /// If the calculation yields a result of no attenuation effect, this value has no effect.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The distance scale of an audio emitter must be greater than zero.</exception>
        public float DistanceScale
        {
            get
            {
                return distanceScale;
            }

            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();

                distanceScale = value;
            }
        }

        private float distanceScale;
    }
}
