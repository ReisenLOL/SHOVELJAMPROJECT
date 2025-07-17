using UnityEngine;

public class CoreController : FluxPump
{
    protected override void Start()
    {
        base.Start();
        canProduce = true;
    }
    protected override void OnDestroy()
    {
        FindFirstObjectByType<GameManager>().GameOver(true);
    }
}
