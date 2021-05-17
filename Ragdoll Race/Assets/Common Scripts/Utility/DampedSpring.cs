using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DampedSpring
{
    public static Vector3 GetDampedSpringForce(Vector3 objectPosition, Vector3 targetPosition, Vector3 relativeVelocity, float mass, float springConstant, float dampingRatio){
        // Calculates the combined force of a spring and damping force on an object, with object velocity being relative to the target's velocity

        Vector3 displacement = objectPosition - targetPosition;

        Vector3 force = (-2 * dampingRatio * relativeVelocity * Mathf.Sqrt(mass * springConstant)) - (springConstant * displacement);

        return force;
    }
    public static Vector3 GetDampedSpringForce(Vector3 objectDisplacement, Vector3 relativeVelocity, float mass, float springConstant, float dampingRatio){
        // Calculates the combined force of a spring and damping force on an object, with object velocity being relative to the target's velocity

        Vector3 force = (-2 * dampingRatio * relativeVelocity * Mathf.Sqrt(mass * springConstant)) - (springConstant * objectDisplacement);

        return force;
    }


    public static Vector3 GetDampedSpringAcceleration(Vector3 objectPosition, Vector3 targetPosition, Vector3 relativeVelocity, float naturalFrequency, float dampingRatio){
        // Calculates the combined force of a spring and damping force on an object, with object velocity being relative to the target's velocity

        Vector3 displacement = objectPosition - targetPosition;

        Vector3 acceleration = (-2 * dampingRatio * naturalFrequency * relativeVelocity) - (Mathf.Pow(naturalFrequency, 2) * displacement);

        return acceleration;
    }
    public static Vector3 GetDampedSpringAcceleration(Vector3 objectDisplacement, Vector3 relativeVelocity, float naturalFrequency, float dampingRatio){
        // Calculates the combined force of a spring and damping force on an object, with object velocity being relative to the target's velocity

        Vector3 acceleration = (-2 * dampingRatio * naturalFrequency * relativeVelocity) - (Mathf.Pow(naturalFrequency, 2) * objectDisplacement);

        return acceleration;
    }


    public static Vector3 GetDampedSpringTorque(Vector3 objectForward, Vector3 targetForward, Vector3 angularVelocity, float springConstant, float dampingConstant){
        // Calculates the combined torque of an angular spring and damping force on an object
        // All rotational vectors are in radians
        // NOTE: only applies torque along axes perpendicular to the forward axis - does not apply roll along given axis. Use overload instead

        Vector3 displacementAxis = Vector3.Cross(targetForward, objectForward).normalized;
        float displacementAngle = Vector3.Angle(objectForward, targetForward) * Mathf.Deg2Rad;

        Vector3 velocityAxis = angularVelocity.normalized;
        float velocityRate = angularVelocity.magnitude;
        
        Vector3 torque = -(dampingConstant * velocityRate * velocityAxis) - (springConstant * displacementAngle * displacementAxis);
        
        return torque;
    }

    public static Vector3 GetDampedSpringTorque(Transform currentTransform, Transform targetTransform, Vector3 angularVelocityRadian, float springConstant, float dampingConstant){
        // Calculates the combined torque of an angular spring and damping force on an object
        // All rotational vectors are in radians

        Quaternion displacementQuaternion = currentTransform.rotation * Quaternion.Inverse(targetTransform.rotation);
        displacementQuaternion.ToAngleAxis(out float displacementAngle, out Vector3 displacementAxis);
        displacementAngle *= Mathf.Deg2Rad;

        // Make sure calculated angles and axes are not NaN or infinity
        //currAxis = currAxis.IsRealNumber() ? currAxis : Vector3.zero;
        //targetAxis = targetAxis.IsRealNumber() ? targetAxis : Vector3.zero;
        //currAngle = currAngle.IsRealNumber() ? currAngle : 0;
        //targetAngle = targetAngle.IsRealNumber() ? targetAngle : 0;

        Vector3 velocityAxis = angularVelocityRadian.normalized;
        float velocityRate = angularVelocityRadian.magnitude;
        
        Vector3 torque = -(dampingConstant * velocityRate * velocityAxis) - (springConstant * displacementAngle * displacementAxis);
        
        return torque;
    }

}
