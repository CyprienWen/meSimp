﻿using System.Linq;
using nnmnist.Networks.Graph;
using nnmnist.Networks.Opts;
using nnmnist.Networks.Units;

namespace nnmnist.Networks
{
    class MLPVar : NetBase
    {
        // multilayer perceptron, top k backprop, with simplification
        // relu is the activation function
        // cross entropy is the cost function

        public readonly DenseUnit[] Layers;
        readonly int _top;
        public override NetType Type => NetType.mlpvar;

        public MLPVar(Config conf, int nInput, int nOuput, OptBase opt) : base(conf, nInput, nOuput, opt)
        {
            Layers = new DenseUnit[Conf.Layers];

            for (var i = 0; i < Layers.Length; i++)
            {
                if (i == 0 && i == Conf.Layers - 1)
                {
                    Layers[i] = new DenseUnit(this, InDim, OutDim);
                }
                else if (i == 0)
                {
                    Layers[i] = new DenseMaskedUnit(this, InDim, HidDim);
                }
                else if (i == Conf.Layers - 1)
                {
                    Layers[i] = new DenseUnit(this, HidDim, OutDim);
                }
                else
                {
                    Layers[i] = new DenseMaskedUnit(this, HidDim, HidDim);
                }
            }
            _top = conf.TrainTop;
        }

        // simplify the layers
        public virtual void Prune(float per)
        {
            foreach (var layer in Layers)
            {
                if (layer is DenseMaskedUnit mlayer)
                {
                    mlayer.Prune(per);
                }
            }
        }

        protected override (Tensor res, Tensor loss) Forward(Flow f, Tensor x, Tensor y)
        {

            var h = new Tensor[Conf.Layers + 1];
            h[0] = x;

            for (var i = 0; i < Conf.Layers; i++)
            {
                var layer = Layers[i];
                if (layer is DenseMaskedUnit mlayer)
                {
                    h[i + 1] = f.Rectifier(mlayer.StepTopK(f, h[i], mlayer.Indices, _top));
                }
                else
                {
                    if (i == Conf.Layers - 1)
                    {
                        h[i + 1] = layer.Step(f, h[i]);
                    }
                    else
                    {
                        h[i + 1] = f.Rectifier(layer.Step(f, h[i]));
                    }
                }
            }

            (var prob, var loss) = f.SoftmaxWithCrossEntropy(h.Last(), y);

            return (prob, loss);
        }


    }
}
