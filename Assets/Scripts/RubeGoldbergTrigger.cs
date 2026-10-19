using UnityEngine;

public class RubeGoldbergTrigger : MonoBehaviour
{
    private RubeGoldbergMachine machine;
    private int station;

    public void Initialize(RubeGoldbergMachine owner, int stationIndex)
    {
        machine = owner;
        station = stationIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (machine == null || other.attachedRigidbody == null ||
            !other.attachedRigidbody.CompareTag("Player"))
            return;

        if (station < 0)
            machine.CompleteMachine();
        else
            machine.ReachStation(station);
    }
}
