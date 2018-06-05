using UnityEngine;
using System.Collections;
#pragma warning disable

namespace Eyesblack.FX {
    [AddComponentMenu("Eyesblack/FX/Close to Terrain")]
    public class CloseToTerrain : MonoBehaviour {
        public enum MeshMode {
            Unsupported = 0,
            MeshRendered,
            ParticleRendered
        }

        public TerrainDetector Detector { set; get; }

        MeshMode _meshMode;
        ParticleSystem _particleSystem;
        ParticleSystem.Particle[] _particles;


        Mesh _mesh;
        Vector3[] _rawVertices;
        Vector3[] _vertices;

        public void ReplaceMesh() {
            _meshMode = MeshMode.Unsupported;

            _particleSystem = GetComponent<ParticleSystem>();
            if (_particleSystem != null) {
                _particles = new ParticleSystem.Particle[_particleSystem.maxParticles];
                ParticleSystemRenderer particleRenderer = GetComponent<ParticleSystemRenderer>();
                if (particleRenderer != null) {
                    if (particleRenderer.renderMode == ParticleSystemRenderMode.Mesh) {
                        if (!particleRenderer.mesh.isReadable) {
                            Debug.LogWarning(name + ": 无法贴地表，因为Mesh数据不可读写！");
                        } else {
                            _meshMode = MeshMode.ParticleRendered;

                            _mesh = Instantiate(particleRenderer.mesh);
                            _mesh.MarkDynamic();

                            _rawVertices = _mesh.vertices;
                            _vertices = _mesh.vertices;

                            particleRenderer.mesh = _mesh;
                        }
                    }
                }            
            } else {
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.enabled) {
                    MeshFilter meshFilter = GetComponent<MeshFilter>();
                    if (meshFilter != null) {
                        if (!meshFilter.sharedMesh.isReadable) {
                            Debug.LogWarning(name + ": 无法贴地表，因为Mesh数据不可读写！");
                        } else {
                            _meshMode = MeshMode.MeshRendered;

                            _mesh = meshFilter.mesh;
                            _mesh.MarkDynamic();
                            _rawVertices = _mesh.vertices;
                            _vertices = _mesh.vertices;
                        }
                    }
                }
            }


            if (_meshMode == MeshMode.Unsupported) {
                enabled = false;
            }
        }

        void UpdateMesh(Matrix4x4 localToWorldMatrix) {
            Matrix4x4 worldToLocalMatrix = localToWorldMatrix.inverse;

            bool needUpdateMesh = false;
            for (int i = 0; i < _vertices.Length; i++) {
                Vector3 vWorld = localToWorldMatrix.MultiplyPoint(_rawVertices[i]);
                float oldY = vWorld.y;
                vWorld.y += Detector.GetHeight(vWorld) + 0.02f;
                if (vWorld.y != oldY) {
                    Vector3 v = worldToLocalMatrix.MultiplyPoint(vWorld);
                    _vertices[i].z = v.z;

                    needUpdateMesh = true;
                }
            }

            if (needUpdateMesh) {
                _mesh.vertices = _vertices;
            }
        }


        void LateUpdate() {
            if (Detector == null || _meshMode == MeshMode.Unsupported)
                return;

            switch (_meshMode) {
                case MeshMode.MeshRendered: {
                        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
                        UpdateMesh(localToWorldMatrix);
                    }
                    break;

                case MeshMode.ParticleRendered: {
                        int numParticlesAlive = _particleSystem.GetParticles(_particles);
                        if (numParticlesAlive > 0) {  //只适用于单个粒子
                            ParticleSystem.Particle particle = _particles[0];
                            Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;

							Vector3 particlePos = _particleSystem.simulationSpace == ParticleSystemSimulationSpace.Local ? particle.position : Vector3.zero;
							Quaternion particleRot = Quaternion.Euler (particle.rotation3D);
							float scale = particle.GetCurrentSize (_particleSystem);
							if (scale == 0)
								scale = 0.0001f;
							Vector3 particleScale = new Vector3 (scale, scale, scale);
							Matrix4x4 matLocal = Matrix4x4.TRS (particlePos, particleRot, particleScale);
							localToWorldMatrix *= matLocal;
              
                            UpdateMesh(localToWorldMatrix);
                        }
                    }
                    break;
            }
        }
    }
}