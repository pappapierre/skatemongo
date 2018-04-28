using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Particles_GatherToPoint : MonoBehaviour {

    public ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;

    public Vector3 targetPoint;
    //public AnimationCurve powerOverLife = new AnimationCurve();

    private void LateUpdate() {
        InitializeIfNeeded();
        int numParticlesAlive = m_System.GetParticles(m_Particles);

        Vector3 dif;
        float age;
        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++) {
            age = (m_Particles[i].startLifetime - m_Particles[i].remainingLifetime) / m_Particles[i].startLifetime;
            dif = (m_Particles[i].position);// + targetPoint));
            m_Particles[i].velocity -= dif * Random.Range(0.9f, 0.99f);
            m_Particles[i].velocity *= 0.94f;
        }

        m_System.SetParticles(m_Particles, numParticlesAlive);
    }

    void InitializeIfNeeded() {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
    }
}

