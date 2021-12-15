using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Client.Animation._3DWall
{
   public  class Particle
    {
        public Point3D Position;//位置
        public double Width;//长方体底面宽
        public double Height;//长方体侧面高
    }

    public class ParticleSystem 
    {
        private readonly List<Particle> _particleList;
        private readonly GeometryModel3D _particleModel;
        private readonly int CUBOIDHEIGHT = 20;
        private readonly int MOUSERADIUS = 1000;
        private int XParticleCount;
        private int YParticleCount;
        public Model3D ParticleModel => _particleModel;

        public ParticleSystem(int amountX, int amountY, Color color)
        {
            XParticleCount = amountX;
            YParticleCount = amountY;

            _particleList = new List<Particle>();
            _particleModel = new GeometryModel3D { Geometry = new MeshGeometry3D() };
            var material = new DiffuseMaterial(new SolidColorBrush(color));
            _particleModel.Material = material;
        }

        public void SpawnParticle(double size)
        {
            // 初始化粒子位置和大小
            for (int ix = 0; ix < XParticleCount; ix++)
            {
                for (int iy = 0; iy < YParticleCount; iy++)
                {
                    var p = new Particle
                    {
                        Position = new Point3D(ix * size, iy * size, 0),
                        Width = size,
                        Height = CUBOIDHEIGHT,
                    };
                    _particleList.Add(p);
                }
            }
        }

        public void Update(Point mp)
        {
            foreach (var p in _particleList)
            {
                //求点到圆心的距离
                double c = Math.Pow(Math.Pow(mp.X - p.Position.X, 2) + Math.Pow(mp.Y - p.Position.Y, 2), 0.5);
                p.Height = (MOUSERADIUS / (c + CUBOIDHEIGHT)) * CUBOIDHEIGHT;
            }
            UpdateGeometry();
        }

        private void UpdateGeometry()
        {
            var positions = new Point3DCollection();
            var indices = new Int32Collection();

            for (var i = 0; i < _particleList.Count; ++i)
            {
                var positionIndex = i * 8;
                var p = _particleList[i];

                var p1 = new Point3D(p.Position.X, p.Position.Y, p.Position.Z);
                var p2 = new Point3D(p.Position.X + p.Width, p.Position.Y, p.Position.Z);
                var p3 = new Point3D(p.Position.X + p.Width, p.Position.Y + p.Width, p.Position.Z);
                var p4 = new Point3D(p.Position.X, p.Position.Y + p.Width, p.Position.Z);
                var p5 = new Point3D(p.Position.X, p.Position.Y, p.Position.Z + p.Height);
                var p6 = new Point3D(p.Position.X + p.Width, p.Position.Y, p.Position.Z + p.Height);
                var p7 = new Point3D(p.Position.X + p.Width, p.Position.Y + p.Width, p.Position.Z + p.Height);
                var p8 = new Point3D(p.Position.X, p.Position.Y + p.Width, p.Position.Z + p.Height);

                positions.Add(p1);
                positions.Add(p2);
                positions.Add(p3);
                positions.Add(p4);
                positions.Add(p5);
                positions.Add(p6);
                positions.Add(p7);
                positions.Add(p8);

                indices.Add(positionIndex);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 7);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 6);
                indices.Add(positionIndex + 7);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 5);
                indices.Add(positionIndex + 6);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 5);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 6);
                indices.Add(positionIndex + 6);
                indices.Add(positionIndex + 5);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 7);
                indices.Add(positionIndex + 7);
                indices.Add(positionIndex + 6);
                indices.Add(positionIndex + 2);
            }

              ((MeshGeometry3D)_particleModel.Geometry).Positions = positions;
            ((MeshGeometry3D)_particleModel.Geometry).TriangleIndices = indices;
        }
    }
}
