using System;
using UnityEngine;
using static UnityEngine.Mathf;


namespace Assets.Scripts.Utils
{
    public class SphericalCoordinates
    {
        // Determine what happen when a limit is reached, repeat or clamp.
        public bool LoopPolar = true, LoopElevation = false;

        private float radius, polar, elevation;
        private readonly float minRadius, maxRadius, minPolar, maxPolar, minElevation, maxElevation;

        public float Radius
        {
            get => radius;
            private set => radius = Clamp(value, minRadius, maxRadius);
        }

        public float Polar
        {
            get => polar;
            private set =>
                polar = LoopPolar ? Repeat( value, maxPolar - minPolar )
                    : Clamp( value, minPolar, maxPolar );
        }

        public float Elevation
        {
            get => elevation;
            private set =>
                elevation = LoopElevation ? Repeat( value, maxElevation - minElevation )
                    : Clamp( value, minElevation, maxElevation );
        }

        public Vector3 ToCartesian
        {
            get
            {
                var a = Radius * Cos(Elevation);
                return new Vector3(a * Cos(Polar), Radius * Sin(Elevation), a * Sin(Polar));
            }
        }


        public SphericalCoordinates(){}
        public SphericalCoordinates(float r, float p, float s,
            float minRadius = 0.1f, float maxRadius = 100f,
            float minPolar = 0f, float maxPolar = PI * 4f,
            float minElevation = 0f, float maxElevation = PI / 2f)
        {
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.minPolar = minPolar;
            this.maxPolar = maxPolar;
            this.minElevation = minElevation;
            this.maxElevation = maxElevation;

            SetRadius(r);
            SetRotation(p, s);
        }

        public SphericalCoordinates(Transform T,
            float minRadius = 0.1f, float maxRadius = 100f,
            float minPolar = 0f, float maxPolar = PI * 4f,
            float minElevation = 0f, float maxElevation = PI / 2f) :
            this(T.position, minRadius, maxRadius, minPolar, maxPolar, minElevation, maxElevation) 
        { }

        public SphericalCoordinates(Vector3 cartesianCoordinate,
            float minRadius = 0.1f, float maxRadius = 100f,
            float minPolar = 0f, float maxPolar = PI * 4f,
            float minElevation = 0f, float maxElevation = PI / 2f)
        {
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.minPolar = minPolar;
            this.maxPolar = maxPolar;
            this.minElevation = minElevation;
            this.maxElevation = maxElevation;

            FromCartesian( cartesianCoordinate );
        }

        public SphericalCoordinates FromCartesian(Vector3 cartesianCoordinate)
        {
            if (Abs(cartesianCoordinate.x) < double.Epsilon)
                cartesianCoordinate.x = Epsilon;
        
            Radius = cartesianCoordinate.magnitude;
            Polar = Atan(cartesianCoordinate.z / cartesianCoordinate.x);
            Elevation = Asin(cartesianCoordinate.y / Radius);

            if (cartesianCoordinate.x < 0f)
                Polar += PI;

            return this;
        }

        public SphericalCoordinates RotatePolarAngle(float x) { return Rotate(x, 0f); }
        public SphericalCoordinates RotateElevationAngle(float x) { return Rotate(0f, x); }
        public SphericalCoordinates Rotate(float newPolar, float newElevation){ return SetRotation( Polar + newPolar, Elevation + newElevation ); }
        public SphericalCoordinates SetPolarAngle(float x) { return SetRotation(x, Elevation); }
        public SphericalCoordinates SetElevationAngle(float x) { return SetRotation(Polar, x); }
        public SphericalCoordinates TranslateRadius(float x) { return SetRadius(Radius + x); }

        public SphericalCoordinates SetRadius(float rad)
        {
            Radius = rad;
            return this;
        }

        public SphericalCoordinates SetRotation(float newPolar, float newElevation)
        {
            Polar = newPolar;
            Elevation = newElevation;

            return this;
        }
    }
}