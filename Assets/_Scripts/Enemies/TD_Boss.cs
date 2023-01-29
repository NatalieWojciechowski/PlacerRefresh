using System;
using System.Collections;
using UnityEngine;

public class TD_Boss : TD_Enemy
{

    private enum BossState
    {
        Init,
        Default,
        Dialogue,
    }
    private BossState bossState;
    private Coroutine dialogueCoroutine;
    [SerializeField]
    private GameObject dialogueBubble;

	public TD_Boss()
	{

	}

    protected override void Start()
    {
        base.Start();
        bossState = BossState.Init;
    }

    protected override void Update()
    {
        base.Update();
    }
    IEnumerator StartDialogue()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        // TODO: Display the Dialogue bubble / Pause the enemy movement until interaction?
        if (dialogueBubble) dialogueBubble.GetComponent<ModelShark.TooltipTrigger>().Popup(5f, gameObject);
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        // TODO: Start attacking towers?? spawn enemies?  
    }

    private protected override void Awake()
    {
        base.Awake();

    }

    protected void OnEnable()
    {
        bossState = BossState.Init;
        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
        dialogueCoroutine = StartCoroutine(StartDialogue());

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
