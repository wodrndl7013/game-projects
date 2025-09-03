using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public LineRenderer trajectoryLineRenderer; // 궤적을 그릴 LineRenderer

    public Transform[] stripPosition;
    public Transform center;
    public Transform idlePosition;
    public Vector3 currentPosition;

    public float maxLenght;
    public float birdPositionOffset;
    public float force;
    public int trajectorySegmentCount = 20; // 궤적을 구성할 점의 개수

    public List<GameObject> birds; // List of pre-registered bird objects

    public AudioClip pullSound; // Sound when pulling the sling
    public AudioClip launchSound; // Sound when launching the bird

    private AudioSource audioSource; // AudioSource component

    private bool isMouseDown;
    private Rigidbody2D bird;
    private Collider2D birdCollider;
    private int currentBirdIndex = 0; // Index to keep track of the current bird
    private List<Vector3> pathPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPosition[0].position);
        lineRenderers[1].SetPosition(0, stripPosition[1].position);

        trajectoryLineRenderer.positionCount = trajectorySegmentCount; // 궤적 점 개수 설정

        audioSource = GetComponent<AudioSource>();

        LoadNextBird();
    }

    void LoadNextBird()
    {
        if (currentBirdIndex < birds.Count)
        {
            bird = birds[currentBirdIndex].GetComponent<Rigidbody2D>();
            birdCollider = bird.GetComponent<Collider2D>();
            birdCollider.enabled = false;

            bird.isKinematic = true;

            ResetStrips();
        }
        else
        {
            Debug.Log("No more birds left to shoot.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseDown && bird != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLenght);

            SetStrips(currentPosition);
            UpdateTrajectoryLine();

            if (birdCollider)
            {
                birdCollider.enabled = true;
            }
        }
        else
        {
            ResetStrips();
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
        PlayPullSound();
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        Shoot();
    }

    void Shoot()
    {
        if (bird != null)
        {
            bird.isKinematic = false;
            Vector3 birdForce = (currentPosition - center.position) * force * -1;
            bird.velocity = birdForce;

            PlayLaunchSound();

            bird = null;
            birdCollider = null;
            currentBirdIndex++;
            Invoke("LoadNextBird", 2);
        }
    }

    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
        trajectoryLineRenderer.positionCount = 0; // 궤적 숨기기
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (bird)
        {
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdPositionOffset;
            bird.transform.right = -dir.normalized;
        }
    }

    void UpdateTrajectoryLine()
    {
        Vector3 velocity = (currentPosition - center.position) * force * -1;
        Vector3[] trajectory = PlotTrajectory(bird.position, velocity, trajectorySegmentCount);

        trajectoryLineRenderer.positionCount = trajectory.Length;
        for (int i = 0; i < trajectory.Length; i++)
        {
            trajectoryLineRenderer.SetPosition(i, trajectory[i]);
        }
    }

    Vector3[] PlotTrajectory(Vector3 start, Vector3 startVelocity, int segments)
    {
        Vector3[] result = new Vector3[segments];
        float timestep = Time.fixedDeltaTime;
        Vector3 gravity = Physics2D.gravity;
        Vector3 position = start;
        Vector3 velocity = startVelocity;

        for (int i = 0; i < segments; i++)
        {
            result[i] = position;
            position += velocity * timestep;
            velocity += gravity * timestep;
        }

        return result;
    }

    void PlayPullSound()
    {
        if (pullSound != null && audioSource != null)
        {
            audioSource.clip = pullSound;
            audioSource.Play();
        }
    }

    void PlayLaunchSound()
    {
        if (launchSound != null && audioSource != null)
        {
            audioSource.clip = launchSound;
            audioSource.Play();
        }
    }
}
