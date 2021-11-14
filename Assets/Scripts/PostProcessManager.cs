using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PostProcessManager : MonoBehaviour
{
    [SerializeField]
    private Material postProcessMaterial;
    [SerializeField]
    private Material standardMaterial;
    [SerializeField]
    private Material deadMaterial;
    [SerializeField] 
    private float deadTimer;
    [SerializeField]
    private RawImage rawRenderer;

    public void Start()
    {
        GameLogic.Logic.SetPostProcessManager(this);
        TransitionToRegularState();
    }

    public void TransitionToDeadState()
    {
        var interloper = 0.0f;
        DOTween.To(() => interloper, value =>
        {
            interloper = value;
            rawRenderer.material.Lerp(postProcessMaterial, deadMaterial, interloper);
        }, 1.0f, deadTimer);
    }

    public void TransitionToRegularState()
    {
        var interloper = 0.0f;
        var currentMaterial = rawRenderer.material;
        DOTween.To(() => interloper, value =>
        {
            interloper = value;
            rawRenderer.material.Lerp(currentMaterial, standardMaterial, interloper);
        }, 1.0f, deadTimer);
    }
}
