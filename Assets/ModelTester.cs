using System.Collections;
using UnityEngine;
using Unity.Sentis;
using UnityEngine.Assertions;

public class ModelTester : MonoBehaviour
{
    public ModelAsset modelAsset;

    private Model model;
    private Worker worker;

    private WebCamTexture webCamTexture;

    // Define constants for the model input dimensions
    private const int MODEL_INPUT_WIDTH = 640;
    private const int MODEL_INPUT_HEIGHT = 640;
    private const int MODEL_INPUT_CHANNELS = 3;

    void Start()
    {
        Application.targetFrameRate = 60;

        CreateModels();

        WebCamDevice[] devices = WebCamTexture.devices;
        Assert.AreNotEqual(devices.Length, 0, "No webcam devices found.");
        Debug.Log($"Number of webcam devices found: {devices.Length}");
        
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"Webcam {i}: {devices[i].name}");
        }
        
        webCamTexture = new WebCamTexture(devices[0].name, 608, 608);
        
        Assert.IsNotNull(webCamTexture, "Failed to initialize WebCamTexture.");
        
        webCamTexture.Play();
        
        Debug.Log("WebCamTexture initialized and started successfully.");

        StartCoroutine(RunModel());
    }

    void Update()
    {
        Assert.IsNotNull(webCamTexture, "webCamTexture is null or not playing");
        Assert.IsTrue(webCamTexture.isPlaying);
        
        string s = WebCamTexture.devices[0].name;
    }

    void OnDisable()
    {
        if (worker != null) worker.Dispose();
        
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            Destroy(webCamTexture);
            webCamTexture = null;
        }
    }

    IEnumerator RunModel()
    {
        while (true)
        {
            var inputTensor = TextureConverter.ToTensor(webCamTexture, MODEL_INPUT_WIDTH, MODEL_INPUT_HEIGHT, MODEL_INPUT_CHANNELS);
        
            // Execute the model
            worker.Schedule(inputTensor);
        
            inputTensor.Dispose();
        
            // Process the output
            var output = worker.PeekOutput() as Tensor<float>;
            var chest = ProcessModelOutput(output.ReadbackAndClone());
            float chestX = chest[0],
                chestY = chest[1],
                chestWidth = chest[2],
                chestHeight = chest[3],
                confidence = chest[4];
            ;
            output.Dispose();
            if (confidence > 0.5f) // Adjust this threshold as needed
            {
                chestSize = 
                    (chestWidth)
                    *
                    (chestHeight)
                    ;
                // Debug.Log(chestSize);
                DetectBreathing(chestSize);
            }
            else
            {
                Debug.Log("No chest detected with high confidence");
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private float chestSize = 0f;

    public float getChestSize()
    {
        return chestSize;
    }

    private void CreateModels()
    {
        Debug.Log("Starting initialization...");
        Assert.IsNotNull(modelAsset, "modelAsset is not assigned. Please assign it in the Inspector.");
        
        Tensor<float> centersToCorners = new Tensor<float>(new TensorShape(4, 4),
            new float[]
            {
                1, 0, 1, 0,
                0, 1, 0, 1,
                -0.5f, 0, 0.5f, 0,
                0, -0.5f, 0, 0.5f
            });
        
        var bboxModel = ModelLoader.Load(modelAsset);
        Assert.IsNotNull(bboxModel, "Failed to load model. ModelLoader.Load returned null.");
        Debug.Log("Model loaded successfully.");

        FunctionalGraph bBoxFilter = new FunctionalGraph();
        
        var inputs = bBoxFilter.AddInputs(bboxModel);
        FunctionalTensor[] outputs = Functional.ForwardWithCopy(bboxModel, inputs);
        FunctionalTensor cornerCoords = Functional.MatMul(
            outputs[0][0, ..4, ..].Transpose(0, 1), 
            Functional.Constant(centersToCorners)
            );
        FunctionalTensor allScores = outputs[0][0, 4.., ..];
        FunctionalTensor classIDs = Functional.ArgMax(allScores, 1);
        FunctionalTensor bestBBox = cornerCoords.IndexSelect(0, classIDs[0]);
        FunctionalTensor bestScore = allScores.IndexSelect(1, classIDs[0]);

        FunctionalTensor output = Functional.Concat(new[] { bestBBox, bestScore });
        model = bBoxFilter.Compile(output);
        
        worker = new Worker(model, BackendType.GPUCompute);
        Assert.IsNotNull(worker, "Failed to create worker. WorkerFactory.CreateWorker returned null.");
        Debug.Log("Worker created successfully.");
        
        PrintModelInfo(model);
    }
    
    private float[] ProcessModelOutput(Tensor<float> output)
    {
        float maxConfidence = 0f;
        float x = 0f, y = 0f, width = 0f, height = 0f;

        float[] chest = new float[5];
        chest[0] = output[0];
        chest[1] = output[1];
        chest[2] = output[2] - output[0];
        chest[3] = output[3] - output[1];
        chest[4] = output[4];
        
        output.Dispose();

        return chest;
    }

    private float minChangeThreshold = 0.01f; // Adjust this value as needed
    private bool isInhaling = false;
    private float breathingThreshold = 0.01f;
    private float pastChestSize = 0f;

    private void DetectBreathing(float currentChestSize)
    {
        float relativeChange = (currentChestSize - pastChestSize) / pastChestSize;

        // Print breathing status
        string breathingStatus;
        if (relativeChange > minChangeThreshold)
        {
            if (!isInhaling) isInhaling = true;
            breathingStatus = "Inhale";
        }
        else if (relativeChange < -minChangeThreshold)
        {
            if (isInhaling) isInhaling = false;
            breathingStatus = "Exhale";
        }
        else
        {
            breathingStatus = "No significant change";
        }

        // Log both the numerical data and the breathing status
        Debug.Log($"Current: {currentChestSize}, Last: {pastChestSize}, Relative Change: {relativeChange}, Status: {breathingStatus}");
        // Debug.Log($"Status: {breathingStatus}");

        pastChestSize = currentChestSize;
    }

    private void PrintModelInfo(Model m)
    {
        Debug.Log("Model info:");
        Debug.Log("Inputs:");
        foreach (var input in m.inputs)
        {
            Debug.Log($"  Name: {input.name}, Shape: [{string.Join(", ", input.shape)}]");
        }
        Debug.Log("Outputs:");
        foreach (var output in m.outputs)
        {
            Debug.Log($"  Name: {output}");
        }
    }
}