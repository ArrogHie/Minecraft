using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;

    public int renderChunk = 5;
    public Material cubeMate;
    public GameObject player;
    public Camera loadCamera;
    public WorldLoadingState loadingState;

    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private ConcurrentDictionary<Vector2Int, Chunk> visChunks = new ConcurrentDictionary<Vector2Int, Chunk>();
    private Vector2Int lastPosition;

    private int seed;

    [Header("DropPrefeb")]
    public GameObject dropPrefeb;

    private void Awake()
    {
        instance = this;
        if (loadingState == null)
        {
            loadingState = GetComponent<WorldLoadingState>();
        }
    }

    private void SetSeed()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        seed = Random.Range(-1000, 1000);
    }

    private void Start()
    {
        SetSeed();
        InitPlayer();
        SetLoadingVisualState(true);

        int totalChunks = (renderChunk * 2 + 1) * (renderChunk * 2 + 1);
        if (loadingState != null)
        {
            loadingState.BeginLoading(totalChunks);
        }

        StartCoroutine(GenerateWorld(totalChunks));
    }

    private void Update()
    {
        StartCoroutine(UpdateWorld());
    }

    void InitPlayer()
    {
        player.SetActive(false);
    }

    void SetLoadingVisualState(bool isLoading)
    {
        if (loadCamera != null)
        {
            loadCamera.gameObject.SetActive(isLoading);
        }
    }

    IEnumerator GenerateWorld(int totalChunks)
    {
        int loadedChunks = 0;
        for (int x = -renderChunk; x <= renderChunk; x++)
        {
            for (int y = -renderChunk; y <= renderChunk; y++)
            {
                loadChunk(x, y);

                loadedChunks++;
                if (loadingState != null)
                {
                    loadingState.SetProgress(loadedChunks, totalChunks);
                }

                yield return null;
            }
        }

        if (loadingState != null)
        {
            loadingState.FinishLoading();
        }

        player.SetActive(true);
        SetLoadingVisualState(false);
        lastPosition = new Vector2Int(0, 0);
    }

    IEnumerator UpdateWorld()
    {
        int playerChunkX = Mathf.FloorToInt(player.transform.position.x / Chunk.chunkSize);
        int playerChunkY = Mathf.FloorToInt(player.transform.position.z / Chunk.chunkSize);
        Vector2Int currentPosition = new Vector2Int(playerChunkX, playerChunkY);
        if (currentPosition != lastPosition)
        {
            lastPosition = currentPosition;

            //foreach (KeyValuePair<Vector2Int, Chunk> entry in visChunks)
            //{
            //    Vector2Int pos = entry.Key;
            //    if (Mathf.Abs(pos.x - playerChunkX) > renderChunk || Mathf.Abs(pos.y - playerChunkY) > renderChunk)
            //    {
            //        UnloadChunk(pos.x, pos.y);
            //        yield return null;
            //    }
            //}

            for (int x = -renderChunk; x <= renderChunk; x++)
            {
                for (int y = -renderChunk; y <= renderChunk; y++)
                {
                    Vector2Int pos = new Vector2Int(playerChunkX + x, playerChunkY + y);
                    if (!visChunks.ContainsKey(pos))
                    {
                        loadChunk(pos.x, pos.y);
                        yield return null;
                    }
                }
            }
        }
    }

    private void loadChunk(int x, int y)
    {
        GameObject chunkObj = new GameObject("Chunk " + x + "," + y);
        chunkObj.transform.parent = this.transform;
        chunkObj.transform.position = new Vector3(x * Chunk.chunkSize, 0, y * Chunk.chunkSize);
        chunkObj.tag = "Chunk";
        chunkObj.layer = LayerMask.NameToLayer("Chunk");

        Chunk chunk = chunkObj.AddComponent<Chunk>();
        Vector2Int pos = new Vector2Int(x, y);
        chunks.Add(pos, chunk);
        visChunks.TryAdd(pos, chunk);
        chunk.InitChunk(cubeMate, seed, pos);
        StartCoroutine(chunk.GenerateChunk());
    }

    private void UnloadChunk(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (chunks.ContainsKey(pos))
        {
            Chunk chunk = chunks[pos];
            visChunks.TryRemove(pos, out _);
            chunks.Remove(pos);
            Destroy(chunk.gameObject);
        }
    }

    public Chunk getChunk(Vector2Int pos)
    {
        if (chunks.ContainsKey(pos))
        {
            return chunks[pos];
        }
        return null;
    }

    public void CreatDrop(Vector3 position, BlockType type, Vector3 velocity = default)
    {
        GameObject dropObj = Instantiate(dropPrefeb, position, Quaternion.identity);
        dropObj.name = "DropItem";

        if (velocity != default)
        {
            Rigidbody rb = dropObj.GetComponent<Rigidbody>();
            rb.velocity = velocity;
        }

        DroppedItem droppedItem = dropObj.GetComponentInChildren<DroppedItem>();
        droppedItem.Init(type);
    }

}
