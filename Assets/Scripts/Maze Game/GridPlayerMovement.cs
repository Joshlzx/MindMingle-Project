using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    [Header("References")]
    public MazeGenerator mazeGenerator;

   
    public LayerMask movementCollisionMask = ~0; 
    public bool visualizePathChecks = true;      

    private bool isMoving = false;
    private MazeNode currentNode;
    private Vector3 targetPosition;

    private Rigidbody rb;
    private PlayerMovement legacyMovement;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        legacyMovement = GetComponent<PlayerMovement>();
        if (legacyMovement != null)
        {
            Debug.LogWarning("Disabling legacy PlayerMovement to avoid conflicts with GridPlayerMovement. Remove it from the prefab if not needed.");
            legacyMovement.enabled = false;
        }
    }

    public void Init()
    {
        if (mazeGenerator == null)
        {
            Debug.LogError("MazeGenerator not assigned!");
            return;
        }

        ConfigureRigidbodyForPhysics();

        
        currentNode = mazeGenerator.GetNodeAtPosition(transform.position);
        if (currentNode == null) Debug.LogWarning("Init: No closest node found at start position.");

        Vector3 startPos;
        if (currentNode != null)
            startPos = GetNodePosition(currentNode);
        else
            startPos = transform.position; 

        if (rb != null) rb.position = startPos;
        transform.position = startPos;
        targetPosition = startPos;
    }

    void Start()
    {
        if (mazeGenerator == null)
        {
            Debug.LogError("MazeGenerator not assigned!");
            return;
        }

        ConfigureRigidbodyForPhysics();

        // Start at closest node
        currentNode = mazeGenerator.GetNodeAtPosition(transform.position);
        if (currentNode == null) Debug.LogWarning("Start: No closest node found at start position.");

        Vector3 startPos;
        if (currentNode != null)
            startPos = GetNodePosition(currentNode);
        else
            startPos = transform.position; 

        if (rb != null) rb.position = startPos;
        transform.position = startPos;
        targetPosition = startPos;
    }

    void ConfigureRigidbodyForPhysics()
    {
        if (rb == null) return;

        
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (isMoving) return;

        if (mazeGenerator == null)
        {
            Debug.LogWarning("Update: mazeGenerator is null.");
            return;
        }

        if (currentNode == null)
        {
            currentNode = mazeGenerator.GetNodeAtPosition(transform.position);
            if (currentNode == null)
            {
                Debug.LogWarning("Update: currentNode is null and couldn't be resolved.");
                return;
            }
        }

        MazeNode nextNode = null;

        // Input mapping: Z = left, X = up, C = down, V = right
        if (Input.GetKeyDown(KeyCode.Z))
            nextNode = TryMove(0); // left
        else if (Input.GetKeyDown(KeyCode.V))
            nextNode = TryMove(1); // right
        else if (Input.GetKeyDown(KeyCode.C))
            nextNode = TryMove(2); // down
        else if (Input.GetKeyDown(KeyCode.X))
            nextNode = TryMove(3); // up

        if (nextNode != null)
        {
            StartCoroutine(MoveToNode(nextNode));
        }
    }

    MazeNode TryMove(int direction)
    {
        if (currentNode == null) return null;

        
        int wallIndexToCheck;
        switch (direction)
        {
            case 0: wallIndexToCheck = 1; break; // left -> left wall
            case 1: wallIndexToCheck = 0; break; // right -> right wall
            case 2: wallIndexToCheck = 3; break; // down -> down wall
            case 3: wallIndexToCheck = 2; break; // up -> up wall
            default: wallIndexToCheck = -1; break;
        }

        // If wall logic blocks movement, do not proceed
        if (wallIndexToCheck >= 0 && currentNode.IsWallActive(wallIndexToCheck))
        {
            // debug exact wall GameObject and its active state
            var wallObj = currentNode.GetWallObject(wallIndexToCheck);
            Debug.Log($"TryMove: Blocked by wall (logical). direction={direction} wallIndex={wallIndexToCheck} node={currentNode.name} wallObj={(wallObj!=null?wallObj.name:"null")} active={(wallObj!=null?wallObj.activeSelf.ToString():"n/a")}");
            return null;
        }

        MazeNode neighbor = mazeGenerator.GetNeighbor(currentNode, direction);
        if (neighbor == null)
        {
            Debug.Log($"TryMove: No neighbor found in direction {direction} from node {currentNode.name}.");
            return null;
        }

        // Pre-move physics check: do not start a move if a collider blocks the spatial path
        Vector3 from = GetNodePosition(currentNode);
        Vector3 to = GetNodePosition(neighbor);
        if (!IsPathClear(from, to))
        {
            Debug.Log($"TryMove: Path physically blocked by collider between {from} -> {to}");
            return null;
        }

        return neighbor;
    }

    
    bool IsPathClear(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return true;

        Collider myCol = GetComponent<Collider>();
        float radius = 0.3f; 
        if (myCol is SphereCollider sc)
        {
            radius = sc.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
        }
        else if (myCol is CapsuleCollider cc)
        {
            radius = cc.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
        }
        else if (myCol != null)
        {
            radius = Mathf.Max(myCol.bounds.extents.x, myCol.bounds.extents.z);
        }

        Vector3 dirNorm = dir.normalized;
        Vector3 origin = from + dirNorm * (radius + 0.01f);
        float castDist = Mathf.Max(0f, dist - (radius + 0.02f));

        if (visualizePathChecks)
        {
            Debug.DrawLine(from, to, Color.green, 1.0f);
            Debug.DrawRay(origin, dirNorm * castDist, Color.yellow, 1.0f);
        }

        
        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, dirNorm, castDist, movementCollisionMask, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            
            if (myCol != null && hit.collider == myCol) continue;
            if (hit.collider.transform.IsChildOf(transform)) continue;

            
            Debug.Log($"IsPathClear: blocked by collider '{hit.collider.name}' on GameObject '{hit.collider.gameObject.name}' (layer={LayerMask.LayerToName(hit.collider.gameObject.layer)}) at distance {hit.distance}");

            if (visualizePathChecks)
            {
                Debug.DrawLine(origin, hit.point, Color.red, 2.0f);
            }

            return false;
        }

        return true;
    }

    IEnumerator MoveToNode(MazeNode node)
    {
        isMoving = true;
        targetPosition = GetNodePosition(node);

        // immediate finish if already at target
        if (Vector3.Distance(GetCurrentPosition(), targetPosition) <= 0.01f)
        {
            FinishMove(node, targetPosition);
            yield break;
        }

        float startTime = Time.time;
        float timeout = 3f;

        if (rb != null)
        {
            while (Vector3.Distance(rb.position, targetPosition) > 0.01f)
            {
                if (Time.time - startTime > timeout)
                {
                    Debug.LogWarning("MoveToNode: Movement timed out. Aborting move and staying at current node.");
                    
                    isMoving = false;
                    yield break;
                }

                Vector3 next = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(next);
                yield return new WaitForFixedUpdate();
            }

            
            rb.MovePosition(targetPosition);
            transform.position = targetPosition;
        }
        else
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                if (Time.time - startTime > timeout)
                {
                    Debug.LogWarning("MoveToNode: Movement timed out. Aborting move and staying at current node.");
                    isMoving = false;
                    yield break;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
        }

        FinishMove(node, targetPosition);
    }

    void FinishMove(MazeNode node, Vector3 pos)
    {
        currentNode = node;
        transform.position = pos;
        if (rb != null) rb.position = pos;
        isMoving = false;
    }

    Vector3 GetCurrentPosition()
    {
        return rb != null ? rb.position : transform.position;
    }

    Vector3 GetNodePosition(MazeNode node)
    {
        if (node == null) return transform.position;
        Vector3 pos = node.transform.position;
        pos.y = transform.position.y; // keep player height
        return pos;
    }

    // Public helper: snap the player to the closest grid node immediately
    public void SnapToClosestNode()
    {
        if (mazeGenerator == null) return;
        currentNode = mazeGenerator.GetNodeAtPosition(transform.position);
        if (currentNode != null)
        {
            StopAllCoroutines();
            isMoving = false;
            Vector3 snapPos = GetNodePosition(currentNode);
            if (rb != null) rb.position = snapPos;
            transform.position = snapPos;
            targetPosition = snapPos;
        }
    }

    void OnDrawGizmos()
    {
        if (mazeGenerator == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetCurrentPosition(), 0.3f);

        if (currentNode != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(GetCurrentPosition(), GetNodePosition(currentNode));
        }
    }
}