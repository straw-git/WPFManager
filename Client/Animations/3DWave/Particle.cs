using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Client.Animations._3DWave
{
    public class Particle
    {
        public Point3D Position;//位置
        public double Size;//尺寸
        public int XIndex;//X位置标识
        public int YIndex;//Y位置标识
    }
    public class ParticleSystem
    {
        private readonly List<Particle> _particleList;
        private readonly GeometryModel3D _particleModel;
        private readonly int SEPARATION = 100;

        public ParticleSystem(int amountX, int amountY, Color color, int Size)
        {
            XParticleCount = amountX;
            YParticleCount = amountY;

            _particleList = new List<Particle>();

            _particleModel = new GeometryModel3D { Geometry = new MeshGeometry3D() };

            var e = new Ellipse
            {
                Width = Size,
                Height = Size
            };
            var b = new RadialGradientBrush();
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, color.R, color.G, color.B), 0.25));
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, color.R, color.G, color.B), 1.0));
            e.Fill = b;
            e.Measure(new Size(Size, Size));
            e.Arrange(new Rect(0, 0, Size, Size));

            Brush brush = null;
            var renderTarget = new RenderTargetBitmap(Size, Size, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(e);
            renderTarget.Freeze();
            brush = new ImageBrush(renderTarget);

            var material = new DiffuseMaterial(brush);
            _particleModel.Material = material;
        }

        public int XParticleCount { get; set; }
        public int YParticleCount { get; set; }
        public Model3D ParticleModel => _particleModel;
        private double _count = 0;

        public void Update()
        {
            // 计算粒子位置及大小
            for (int ix = 0; ix < XParticleCount; ix++)
            {
                for (int iy = 0; iy < YParticleCount; iy++)
                {
                    foreach (var p in _particleList)
                    {
                        if (p.XIndex == ix && p.YIndex == iy)
                        {
                            p.Position.Z = (Math.Sin((ix + _count) * 0.3) * 100) + (Math.Sin((iy + _count) * 0.5) * 100);
                            p.Size = (Math.Sin((ix + _count) * 0.3) + 1) * 8 + (Math.Sin((iy + _count) * 0.5) + 1) * 8;
                        }
                    }
                }
            }
            _count += 0.1;

            UpdateGeometry();
        }

        private void UpdateGeometry()
        {
            var positions = new Point3DCollection();
            var indices = new Int32Collection();
            var texcoords = new PointCollection();

            for (var i = 0; i < _particleList.Count; ++i)
            {
                var positionIndex = i * 4;
                var indexIndex = i * 6;
                var p = _particleList[i];

                var p1 = new Point3D(p.Position.X, p.Position.Y, p.Position.Z);
                var p2 = new Point3D(p.Position.X, p.Position.Y + p.Size, p.Position.Z);
                var p3 = new Point3D(p.Position.X + p.Size, p.Position.Y + p.Size, p.Position.Z);
                var p4 = new Point3D(p.Position.X + p.Size, p.Position.Y, p.Position.Z);

                positions.Add(p1);
                positions.Add(p2);
                positions.Add(p3);
                positions.Add(p4);

                var t1 = new Point(0.0, 0.0);
                var t2 = new Point(0.0, 1.0);
                var t3 = new Point(1.0, 1.0);
                var t4 = new Point(1.0, 0.0);

                texcoords.Add(t1);
                texcoords.Add(t2);
                texcoords.Add(t3);
                texcoords.Add(t4);

                indices.Add(positionIndex);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 2);
            }

         ((MeshGeometry3D)_particleModel.Geometry).Positions = positions;
            ((MeshGeometry3D)_particleModel.Geometry).TriangleIndices = indices;
            ((MeshGeometry3D)_particleModel.Geometry).TextureCoordinates = texcoords;
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
                        Position = new Point3D(ix * SEPARATION - ((XParticleCount * SEPARATION) / 2), iy * SEPARATION - ((YParticleCount * SEPARATION) / 2), 0),
                        Size = size,
                        XIndex = ix,
                        YIndex = iy,
                    };
                    _particleList.Add(p);
                }
            }
        }
    }
}
