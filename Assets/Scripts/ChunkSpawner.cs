using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private int chunkWidth;
    [SerializeField] private float generateAheadDistance;
    [SerializeField] private int MaxChunkVisible;
    [SerializeField] private int initialChunk;

    [Header("References")]
    [SerializeField] private GameObject[] chunkPrefab;
    [SerializeField] private Transform player;

    private List<GameObject> activeChunks = new List<GameObject>();
    private List<GameObject> availableChunkPrefabs; // لیست قابل حذف

    private int currentChunkIndex;
    private float chunkWorldWidth;

    void Start()
    {
        if (chunkPrefab.Length == 0)
            return;

        chunkWorldWidth = chunkWidth;

        // تبدیل آرایه به لیست تا بتوان از آن حذف کرد
        availableChunkPrefabs = new List<GameObject>(chunkPrefab);

        for (int i = 0; i < initialChunk; i++)
        {
            GameObject newChunk = GenerateChunk(i);
            if (newChunk != null)
                activeChunks.Add(newChunk);
        }
    }

    void Update()
    {
        float playerPosX = player.position.x;
        float furthestChunkEndX = (currentChunkIndex - 1 + activeChunks.Count) * chunkWorldWidth;

        if (playerPosX + generateAheadDistance > furthestChunkEndX)
        {
            GameObject newChunk = GenerateChunk(currentChunkIndex - 1 + activeChunks.Count);
            if (newChunk != null)
                activeChunks.Add(newChunk);
        }

        if (activeChunks.Count > MaxChunkVisible)
        {
            GameObject oldestChunk = activeChunks[0];
            activeChunks.RemoveAt(0);
            Destroy(oldestChunk);
            currentChunkIndex++;
        }
    }

    GameObject GenerateChunk(int currentChunkIndex)
    {
        if (availableChunkPrefabs.Count == 0)
        {
            Debug.LogWarning("تمام چانک‌ها ساخته شده‌اند. چانک جدیدی وجود ندارد.");
            return null;
        }

        int randomIndex = Random.Range(0, availableChunkPrefabs.Count);
        GameObject selectedChunk = availableChunkPrefabs[randomIndex];
        availableChunkPrefabs.RemoveAt(randomIndex); // حذف چانک از لیست

        GameObject chunk = Instantiate(selectedChunk, transform);
        chunk.name = "Chunk_" + currentChunkIndex + "_Type_" + randomIndex;
        float xPos = currentChunkIndex * chunkWorldWidth;
        chunk.transform.position = new Vector3(xPos, 0, 0);
        return chunk;
    }
}
