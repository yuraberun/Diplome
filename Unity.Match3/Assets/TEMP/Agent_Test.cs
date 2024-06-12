using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Agent_Test : Agent
{
    [SerializeField] private Transform _env;
    [SerializeField] private Transform _target;
    [SerializeField] private SpriteRenderer _bg;

    private float _speed;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.5f, -1.5f), Random.Range(-3.5f, 3.5f), 0f);
        _target.localPosition = new Vector3(Random.Range(1.5f, 3.5f), Random.Range(-3.5f, 3.5f), 0f);
        _speed = Random.Range(3f, 8f);
        _env.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        transform.rotation = Quaternion.identity;
        _target.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)_target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        float movementSpeed = _speed;
        transform.localPosition += new Vector3(moveX, moveY) * movementSpeed * Time.deltaTime;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            _bg.color = Color.red;
            AddReward(-2f);
            EndEpisode();
        }
        else if (collision.tag == "Target")
        {
            _bg.color = Color.green;
            AddReward(10f);
            EndEpisode();
        }
    }
}
