using UnityEngine;

public class Mission : MonoBehaviour
{
    public MissionTracker missionTracker;
    public int missionID;
    public int counter;

    public string ReturnMissionTitle()
    {
        return missionTracker.RetrieveMissionTitle(missionID);
    }

    public string ReturnMissionDescription()
    {
        return missionTracker.RetrieveMissionDescription(missionID);
    }

    public int ReturnMissionGoal()
    {
        return missionTracker.RetrieveMissionGoal(missionID);
    }

    public char ReturnMissionRewardType()
    {
        return missionTracker.RetrieveMissionRewardType(missionID);
    }

    public int ReturnMissionRewardValue()
    {
        return missionTracker.RetrieveMissionRewardValue(missionID);
    }

    public bool CheckMissionComplete()
    {
        return (counter >= ReturnMissionGoal());
    }

    /*public int ReturnMissionObjective()
    {
        return missionTracker.RetrieveMissionObjective(missionID);
    }*/

    public void IncrementCounter(int counterIncrement)
    {
        counter += counterIncrement;
    }

    public bool CheckCompletion()
    {
        return true;
    }
}