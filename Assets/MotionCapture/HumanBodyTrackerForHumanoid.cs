using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HumanBodyTrackerForHumanoid : MonoBehaviour
{
    public UnityEvent OnTrackStart = new UnityEvent();
    
    [SerializeField]
    [Tooltip("The Skeleton prefab to be controlled.")]
    GameObject m_SkeletonPrefab;

    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce body tracking events.")]
    ARHumanBodyManager m_HumanBodyManager;

    /// <summary>
    /// Get/Set the <c>ARHumanBodyManager</c>.
    /// </summary>
    public ARHumanBodyManager humanBodyManager
    {
        get { return m_HumanBodyManager; }
        set { m_HumanBodyManager = value; }
    }

    /// <summary>
    /// Get/Set the skeleton prefab.
    /// </summary>
    public GameObject skeletonPrefab
    {
        get { return m_SkeletonPrefab; }
        set { m_SkeletonPrefab = value; }
    }

    Dictionary<TrackableId, BoneController> m_SkeletonTracker = new Dictionary<TrackableId, BoneController>();

    void OnEnable()
    {
        Debug.Assert(m_HumanBodyManager != null, "Human body manager is required.");
        m_HumanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        if (m_HumanBodyManager != null)
            m_HumanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        BoneController boneController;

        foreach (var humanBody in eventArgs.added)
        {
            if (!m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
            {
                Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
                var newSkeletonGO = Instantiate(m_SkeletonPrefab, humanBody.transform);
                boneController = newSkeletonGO.GetComponent<BoneController>();
                m_SkeletonTracker.Add(humanBody.trackableId, boneController);
                newSkeletonGO.layer = 1;
                OnTrackStart.Invoke();
            }

            boneController.InitializeSkeletonJoints();
            boneController.ApplyBodyPose(humanBody);
        }

        foreach (var humanBody in eventArgs.updated)
        {
            if (m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
            {
                boneController.ApplyBodyPose(humanBody);
            }
        }

        foreach (var humanBody in eventArgs.removed)
        {
            Debug.Log($"Removing a skeleton [{humanBody.trackableId}].");
            if (m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
            {
                Destroy(boneController.gameObject);
                m_SkeletonTracker.Remove(humanBody.trackableId);
            }
        }
    }

    [SerializeField] private Animator _targetAnimator;
    private Transform _originRobot;
    private Animator _originAnimator;
    private HumanPoseHandler _targetHandler;
    private HumanPoseHandler _originHandler;
    private HumanPose _humanPose;

    private void Start()
    {
        _targetHandler = new HumanPoseHandler(_targetAnimator.avatar, _targetAnimator.transform);
    }

    private void Update()
    {
        if (_originRobot == null)
        {
            _originRobot = FindObjectOfType<BoneController>()?.transform;
            return;
        }

        if (_originHandler == null)
        {
            _originAnimator = _originRobot.GetComponent<Animator>();
            _originHandler = new HumanPoseHandler(_originAnimator.avatar, _originAnimator.transform);
            _originAnimator = _originRobot.GetComponent<Animator>();
            _targetAnimator.transform.SetParent(_originRobot);
            _targetAnimator.transform.localPosition = Vector3.zero;
            _targetAnimator.transform.localEulerAngles = Vector3.zero;
        }
        
        _originHandler.GetHumanPose(ref _humanPose);
        _targetHandler.SetHumanPose(ref _humanPose);

        _targetAnimator.rootPosition = _originAnimator.rootPosition;
        _targetAnimator.rootRotation = _originAnimator.rootRotation;

    }
}
