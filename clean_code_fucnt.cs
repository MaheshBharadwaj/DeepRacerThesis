

// private List<Vector3> OldCheckpointPositions -> Old checkpoints vector for resetting position after adversarial training

public void Awake()
// Initializer function called at the start of an episode
{
    carId = getIDNumber(this.name);
    current = this;
    Transform checkpointsTransform = this.transform.Find("Checkpoints");
    OldCheckpointPositions = new List<Vector3>();
    checkpointSingleList = new List<CheckpointSingle>(); //for adversarial training
    foreach (Transform checkpointSingleTransform in checkpointsTransform)
    {
        CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
        checkpointSingle.setTrackCheckpoints(this);
        checkpointSingleList.Add(checkpointSingle);

        //appending positions of checkpoints to be saved -> used when resetting checkpoints at epsisode emd
        OldCheckpointPositions.Add(checkpointSingle.gameObject.transform.localPosition); 
        if (checkpointSingle.gameObject.TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.material = incorrectCheckpointMaterial;
        }
    }
}

public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle, int carID)
{
    Random rnd = new Random();
    if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
    {
        PlayerCorrectCheckpoint(carID);
        if (nextCheckpointSingleIndex == 0 && gameStarted)
        {
            PlayerLapCompleted(carID);
            return;
        }
        if (!gameStarted)
        {
            gameStarted = true;
        }
        // This checkpoint is incorrect now
        checkpointSingleList[nextCheckpointSingleIndex].gameObject.tag = "IncorrectCheckpointTag";
        if (checkpointSingleList[nextCheckpointSingleIndex].gameObject.TryGetComponent(out MeshRenderer meshRendererOld))
        {
            meshRendererOld.material = incorrectCheckpointMaterial;
        }
        // Obtain next checkpoint index
        nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
        // Adversarial training part -> Adding random noise to checkpoint position
        if (isAdversarialTraining)
        {
            Debug.Log("Before: " +checkpointSingleList[nextCheckpointSingleIndex].gameObject.transform.localPosition);
            checkpointSingleList[nextCheckpointSingleIndex].gameObject.transform.localPosition += new Vector3((float)rnd.Next(-4,4)+(float)rnd.NextDouble()*1f,0f,(float)rnd.Next(-4,4)+(float)rnd.NextDouble()*1f);
            Debug.Log("After: " + checkpointSingleList[nextCheckpointSingleIndex].gameObject.transform.localPosition);
        }
        // Set the tag for correct checkpoint and material for correct checkpoint
        checkpointSingleList[nextCheckpointSingleIndex].gameObject.tag = "CorrectCheckpointTag";
        if (checkpointSingleList[nextCheckpointSingleIndex].gameObject.TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.material = correctCheckpointMaterial;
        }
    }
    else
    {
        PlayerWrongCheckpoint(carID);
    }
}

public void ResetCheckpoint()
{
    nextCheckpointSingleIndex = 0;
    int i = 0; //for resetting checkpoint
    foreach (CheckpointSingle checkpointSingle in checkpointSingleList)
    {
        // resetting checkpoint at end of episode (after adversarial training)
        checkpointSingle.gameObject.transform.localPosition = OldCheckpointPositions[i];
        i++;
        checkpointSingle.gameObject.tag = "IncorrectCheckpointTag";
        if (checkpointSingle.gameObject.TryGetComponent(out MeshRenderer meshRenderer2))
        {
            meshRenderer2.material = incorrectCheckpointMaterial;
        }
    }
    checkpointSingleList[0].gameObject.tag = "CorrectCheckpointTag";
    if (checkpointSingleList[0].gameObject.TryGetComponent(out MeshRenderer meshRenderer))
    {
        meshRenderer.material = correctCheckpointMaterial;
    }
}


