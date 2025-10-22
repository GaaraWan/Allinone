namespace Allinone.ZGa.Mvc.Model.MarkReferenceSystemModel
{
    using System;
    using System.Drawing;

    /// <summary>
    /// 基于两个MARK点的坐标系转换器
    /// </summary>
    public class MarkCoordinateSystem
    {
        /// <summary>
        /// MARK点A（作为原点）
        /// </summary>
        public PointF MarkA { get; private set; }

        /// <summary>
        /// MARK点B（用于确定X轴方向）
        /// </summary>
        public PointF MarkB { get; private set; }

        /// <summary>
        /// 坐标系旋转角度（弧度）
        /// </summary>
        public double RotationAngle { get; private set; }

        /// <summary>
        /// 坐标系缩放比例
        /// </summary>
        public double ScaleFactor { get; private set; }

        /// <summary>
        /// 是否已初始化坐标系
        /// </summary>
        public bool IsInitialized { get; private set; }

        public MarkCoordinateSystem()
        {
            IsInitialized = false;
            ScaleFactor = 1.0;
        }

        /// <summary>
        /// 使用两个MARK点初始化坐标系
        /// </summary>
        /// <param name="markA">MARK点A（作为原点）</param>
        /// <param name="markB">MARK点B（用于确定X轴方向）</param>
        public void Initialize(PointF markA, PointF markB)
        {
            MarkA = markA;
            MarkB = markB;

            // 计算MARK点A到B的向量
            float dx = markB.X - markA.X;
            float dy = markB.Y - markA.Y;

            // 计算原始距离（用于缩放计算）
            double originalDistance = Math.Sqrt(dx * dx + dy * dy);

            // 计算旋转角度
            RotationAngle = Math.Atan2(dy, dx);

            // 可以在这里设置期望的标准距离，如果不设置则使用实际距离
            // double expectedDistance = 100.0; // 例如期望距离为100个单位
            // ScaleFactor = expectedDistance / originalDistance;

            // 这里我们使用1:1比例，不进行缩放
            ScaleFactor = 1.0;

            IsInitialized = true;

            Console.WriteLine($"坐标系初始化完成：");
            Console.WriteLine($"原点: ({MarkA.X}, {MarkA.Y})");
            Console.WriteLine($"X轴方向点: ({MarkB.X}, {MarkB.Y})");
            Console.WriteLine($"旋转角度: {RotationAngle * 180 / Math.PI:F2}°");
            Console.WriteLine($"缩放比例: {ScaleFactor:F4}");
        }

        /// <summary>
        /// 将世界坐标系中的点转换到MARK坐标系
        /// </summary>
        /// <param name="worldPoint">世界坐标系中的点</param>
        /// <returns>MARK坐标系中的相对位置</returns>
        public PointF WorldToMarkCoordinates(PointF worldPoint)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("坐标系未初始化，请先调用Initialize方法");
            }

            // 1. 平移：将原点移动到MARK点A
            float translatedX = worldPoint.X - MarkA.X;
            float translatedY = worldPoint.Y - MarkA.Y;

            // 2. 旋转：将坐标系旋转到MARK坐标系
            double cosAngle = Math.Cos(-RotationAngle); // 反向旋转
            double sinAngle = Math.Sin(-RotationAngle);

            double markX = translatedX * cosAngle - translatedY * sinAngle;
            double markY = translatedX * sinAngle + translatedY * cosAngle;

            // 3. 应用缩放
            markX *= ScaleFactor;
            markY *= ScaleFactor;

            return new PointF((float)markX, (float)markY);
        }

        /// <summary>
        /// 将MARK坐标系中的点转换回世界坐标系
        /// </summary>
        /// <param name="markPoint">MARK坐标系中的点</param>
        /// <returns>世界坐标系中的位置</returns>
        public PointF MarkToWorldCoordinates(PointF markPoint)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("坐标系未初始化，请先调用Initialize方法");
            }

            // 1. 反向缩放
            double scaledX = markPoint.X / ScaleFactor;
            double scaledY = markPoint.Y / ScaleFactor;

            // 2. 反向旋转
            double cosAngle = Math.Cos(RotationAngle);
            double sinAngle = Math.Sin(RotationAngle);

            double worldX = scaledX * cosAngle - scaledY * sinAngle;
            double worldY = scaledX * sinAngle + scaledY * cosAngle;

            // 3. 反向平移
            worldX += MarkA.X;
            worldY += MarkA.Y;

            return new PointF((float)worldX, (float)worldY);
        }

        /// <summary>
        /// 计算点到MARK点A的距离
        /// </summary>
        public double DistanceToMarkA(PointF worldPoint)
        {
            var markCoords = WorldToMarkCoordinates(worldPoint);
            return Math.Sqrt(markCoords.X * markCoords.X + markCoords.Y * markCoords.Y);
        }

        /// <summary>
        /// 获取坐标系信息
        /// </summary>
        public string GetCoordinateSystemInfo()
        {
            if (!IsInitialized)
                return "坐标系未初始化";

            return $"原点: ({MarkA.X:F2}, {MarkA.Y:F2}), " +
                   $"X轴方向: ({MarkB.X:F2}, {MarkB.Y:F2}), " +
                   $"角度: {RotationAngle * 180 / Math.PI:F2}°, " +
                   $"缩放: {ScaleFactor:F4}";
        }
    }
}
