using System.Collections.Generic;


public class Genome
{
    float           _fitness;

    int             _id;

    List<float>     _weights;



    public Genome()
    {
        _fitness = 0.0f;
        _id = 0;
        _weights = new List<float>();
    }


    public float fitness
    {
        get { return _fitness; }
        set { _fitness = value; }
    }

    public int id
    {
        get { return _id; }
        set { _id = value; }
    }

    public List<float> weights
    {
        get { return _weights; }
        set { _weights = value; }
    }
}
