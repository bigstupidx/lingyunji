//
//SpringCollider for unity-chan!
//
//Original Script is here:
//ricopin / SpringCollider.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
using UnityEngine;
using System.Collections;
using Eyesblack;

namespace UnityChan {
    public class SpringCollider : MonoBehaviour {
        public enum Shape {
            Sphere = 0,
            Capsule,
        }

        public enum Direction { X = 0, Y, Z }

        public Shape shape;
        public Vector3 center;
        public float radius = 0.5f;

        public float height = 2;
        public Direction direction = Direction.Y;

        public bool debug = true;

        Matrix4x4 localToWorldMatrix;
        Matrix4x4 worldToLocalMatrix;
        Matrix4x4 colliderMatrix;
        Matrix4x4 colliderMatrixInverse;

        void Awake() {
            UpdateColliderMatrix();
        }


        void UpdateColliderMatrix() {
            if (shape == Shape.Capsule) {
                if (direction == Direction.X)
                    colliderMatrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, 90), Vector3.one);
                else if (direction == Direction.Y)
                    colliderMatrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
                else
                    colliderMatrix = Matrix4x4.TRS(center, Quaternion.Euler(90, 0, 0), Vector3.one);
                colliderMatrixInverse = colliderMatrix.inverse;
            } else {
                //Sphere模式下没有计算Inverser矩阵
                colliderMatrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
            }
        }

        void UpdateMatrix() {
#if UNITY_EDITOR
            UpdateColliderMatrix();
#endif

            localToWorldMatrix = transform.localToWorldMatrix;
            localToWorldMatrix *= colliderMatrix;

            if (shape == Shape.Capsule) {
                worldToLocalMatrix = transform.worldToLocalMatrix;
                worldToLocalMatrix = colliderMatrixInverse * worldToLocalMatrix;
            }
        }


        public bool CheckCollision(Vector3 sphereWorldPosition, float sphereRadius, out Vector3 centerWorldPos) {
            centerWorldPos = Vector3.zero;

            UpdateMatrix();

            if (shape == Shape.Capsule)
                return CheckCapsuleCollision(sphereWorldPosition, sphereRadius, ref centerWorldPos);
            else
                return CheckSphereCollision(sphereWorldPosition, sphereRadius, ref centerWorldPos);
        }

        bool CheckCapsuleCollision(Vector3 sphereWorldPosition, float sphereRadius, ref Vector3 centerWorldPos) {
            bool collision = false;
            Vector3 center = Vector3.zero;

            float capsuleHalfHeight = height / 2;

            Vector3 spherePosLocal = worldToLocalMatrix.MultiplyPoint(sphereWorldPosition);
            spherePosLocal *= this.transform.lossyScale.x;
            if (spherePosLocal.y > -capsuleHalfHeight - sphereRadius && spherePosLocal.y < capsuleHalfHeight + sphereRadius) {
                float distSq = radius + sphereRadius;
                distSq *= distSq;

                float cylinderHalfHeight = Mathf.Max(0, capsuleHalfHeight - radius);
                if (cylinderHalfHeight > 0 && spherePosLocal.y > -cylinderHalfHeight && spherePosLocal.y < cylinderHalfHeight) {
                    if (Vector2.SqrMagnitude(new Vector2(spherePosLocal.x, spherePosLocal.z)) < distSq) {
                        collision = true;
                        center = new Vector3(0, spherePosLocal.y, 0);
                    }
                } else {
                    if (spherePosLocal.y > cylinderHalfHeight) {
                        if (Vector3.SqrMagnitude(spherePosLocal - new Vector3(0, cylinderHalfHeight, 0)) < distSq) {
                            collision = true;
                            center = new Vector3(0, cylinderHalfHeight, 0);
                        }
                    } else {
                        if (Vector3.SqrMagnitude(spherePosLocal - new Vector3(0, -cylinderHalfHeight, 0)) < distSq) {
                            collision = true;
                            center = new Vector3(0, -cylinderHalfHeight, 0);
                        }
                    }
                }
            }

            if (collision) {
                centerWorldPos = localToWorldMatrix.MultiplyPoint(center);
                return true;
            }

            return false;
        }

        bool CheckSphereCollision(Vector3 sphereWorldPosition, float sphereRadius, ref Vector3 centerWorldPos) {
            float distSq = radius + sphereRadius;
            distSq *= distSq;

            Vector3 colliderWorldPos = new Vector3(localToWorldMatrix.m03, localToWorldMatrix.m13, localToWorldMatrix.m23);
            if (Vector3.SqrMagnitude(colliderWorldPos - sphereWorldPosition) < distSq) {
                centerWorldPos = colliderWorldPos;
                return true;
            }

            return false;
        }


        private void OnDrawGizmos() {
            if (debug) {
                UpdateMatrix();

                Gizmos.color = Color.green;
                Gizmos.matrix = localToWorldMatrix;

                if (shape == Shape.Capsule) {
                    float h = Mathf.Max(height, radius * 2);
                    DebugExtension.DrawCapsule(new Vector3(0, -h / 2, 0), new Vector3(0, h / 2, 0), Gizmos.color, radius);
                } else {
                    Gizmos.DrawWireSphere(Vector3.zero, radius);
                }
            }
        }
    }
}