using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Allinone.ZGa.Mvc.Model.MarkReferenceSystemModel
{
    using System;

    public struct Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X:F3}, {Y:F3})";
    }

    public static class RelativeDistanceCalculator
    {
        /// <summary>
        /// 以点1为起点，计算目标点相对于点1到点2连线的局部坐标（水平和垂直距离）
        /// </summary>
        /// <param name="targetPoint">目标点</param>
        /// <param name="point1">起点</param>
        /// <param name="point2">方向点</param>
        /// <returns>(水平距离, 垂直距离) - 水平距离沿点1->点2方向，垂直距离垂直于该方向</returns>
        public static (double horizontal, double vertical) CalculateRelativeDistances(
            Point2D targetPoint, Point2D point1, Point2D point2)
        {
            // 计算方向向量（从点1指向点2）
            double directionX = point2.X - point1.X;
            double directionY = point2.Y - point1.Y;

            // 如果两点重合，无法确定方向
            if (Math.Abs(directionX) < 1e-10 && Math.Abs(directionY) < 1e-10)
            {
                throw new ArgumentException("点1和点2不能重合");
            }

            // 计算目标点相对于点1的向量
            double relativeX = targetPoint.X - point1.X;
            double relativeY = targetPoint.Y - point1.Y;

            // 计算方向向量的长度
            double directionLength = Math.Sqrt(directionX * directionX + directionY * directionY);

            // 计算方向向量的单位向量
            double unitX = directionX / directionLength;
            double unitY = directionY / directionLength;

            // 计算垂直方向的单位向量（旋转90度）
            double perpendicularX = -unitY;
            double perpendicularY = unitX;

            // 计算水平距离（在点1->点2方向上的投影）
            double horizontalDistance = relativeX * unitX + relativeY * unitY;

            // 计算垂直距离（在垂直方向上的投影）
            double verticalDistance = relativeX * perpendicularX + relativeY * perpendicularY;

            return (horizontalDistance, verticalDistance);
        }

        /// <summary>
        /// 计算详细的相对位置信息
        /// </summary>
        public static RelativePositionResult CalculateDetailedRelativePosition(
            Point2D targetPoint, Point2D point1, Point2D point2)
        {
            var (horizontal, vertical) = CalculateRelativeDistances(targetPoint, point1, point2);

            // 计算投影点坐标
            Point2D projectionPoint = CalculateProjectionPoint(targetPoint, point1, point2);

            // 判断位置关系
            string horizontalPosition = horizontal >= 0 ? "前方" : "后方";
            string verticalPosition = vertical >= 0 ? "右侧" : "左侧";

            return new RelativePositionResult
            {
                HorizontalDistance = horizontal,
                VerticalDistance = vertical,
                ProjectionPoint = projectionPoint,
                HorizontalPosition = horizontalPosition,
                VerticalPosition = verticalPosition,
                DistanceFromPoint1 = Math.Sqrt(Math.Pow(targetPoint.X - point1.X, 2) +
                                             Math.Pow(targetPoint.Y - point1.Y, 2))
            };
        }

        /// <summary>
        /// 计算目标点在点1->点2方向上的投影点
        /// </summary>
        private static Point2D CalculateProjectionPoint(Point2D targetPoint, Point2D point1, Point2D point2)
        {
            double directionX = point2.X - point1.X;
            double directionY = point2.Y - point1.Y;

            double relativeX = targetPoint.X - point1.X;
            double relativeY = targetPoint.Y - point1.Y;

            double directionLengthSquared = directionX * directionX + directionY * directionY;

            // 计算投影参数
            double t = (relativeX * directionX + relativeY * directionY) / directionLengthSquared;

            // 计算投影点坐标
            double projectionX = point1.X + t * directionX;
            double projectionY = point1.Y + t * directionY;

            return new Point2D(projectionX, projectionY);
        }
    }
    public class RelativePositionResult
    {
        /// <summary>
        /// 水平距离（沿点1->点2方向，正值为前方，负值为后方）
        /// </summary>
        public double HorizontalDistance { get; set; }

        /// <summary>
        /// 垂直距离（垂直于点1->点2方向，正值为右侧，负值为左侧）
        /// </summary>
        public double VerticalDistance { get; set; }

        /// <summary>
        /// 目标点在点1->点2方向上的投影点
        /// </summary>
        public Point2D ProjectionPoint { get; set; }

        /// <summary>
        /// 水平位置描述（前方/后方）
        /// </summary>
        public string HorizontalPosition { get; set; }

        /// <summary>
        /// 垂直位置描述（右侧/左侧）
        /// </summary>
        public string VerticalPosition { get; set; }

        /// <summary>
        /// 目标点到点1的直线距离
        /// </summary>
        public double DistanceFromPoint1 { get; set; }

        public override string ToString()
        {
            return $"水平距离: {HorizontalDistance:F3} ({HorizontalPosition}), " +
                   $"垂直距离: {VerticalDistance:F3} ({VerticalPosition}), " +
                   $"投影点: {ProjectionPoint}, " +
                   $"距起点距离: {DistanceFromPoint1:F3}";
        }
    }
    public static class CoordinateTransformer
    {
        /// <summary>
        /// 将全局坐标转换为以点1为原点的局部坐标
        /// </summary>
        /// <param name="globalPoint">全局坐标点</param>
        /// <param name="origin">局部坐标系原点（点1）</param>
        /// <param name="xAxisPoint">X轴方向点（点2）</param>
        /// <returns>局部坐标 (X沿点1->点2方向, Y垂直方向)</returns>
        public static Point2D GlobalToLocal(Point2D globalPoint, Point2D origin, Point2D xAxisPoint)
        {
            var (localX, localY) = RelativeDistanceCalculator.CalculateRelativeDistances(
                globalPoint, origin, xAxisPoint);

            return new Point2D(localX, localY);
        }

        /// <summary>
        /// 将局部坐标转换回全局坐标
        /// </summary>
        public static Point2D LocalToGlobal(Point2D localPoint, Point2D origin, Point2D xAxisPoint)
        {
            double directionX = xAxisPoint.X - origin.X;
            double directionY = xAxisPoint.Y - origin.Y;

            double directionLength = Math.Sqrt(directionX * directionX + directionY * directionY);

            if (directionLength < 1e-10)
                throw new ArgumentException("原点与X轴点不能重合");

            // X轴单位向量
            double unitX = directionX / directionLength;
            double unitY = directionY / directionLength;

            // Y轴单位向量（旋转90度）
            double perpendicularX = -unitY;
            double perpendicularY = unitX;

            // 计算全局坐标
            double globalX = origin.X + localPoint.X * unitX + localPoint.Y * perpendicularX;
            double globalY = origin.Y + localPoint.X * unitY + localPoint.Y * perpendicularY;

            return new Point2D(globalX, globalY);
        }
    }

    // 工业应用示例
    public class AlignmentCalculator
    {
        private Point2D _referencePoint1;
        private Point2D _referencePoint2;

        public AlignmentCalculator(Point2D point1, Point2D point2)
        {
            _referencePoint1 = point1;
            _referencePoint2 = point2;
        }

        /// <summary>
        /// 计算目标点相对于基准的位置偏差
        /// </summary>
        public AlignmentResult CalculateAlignment(Point2D targetPoint)
        {
            var relativePos = RelativeDistanceCalculator.CalculateDetailedRelativePosition(
                targetPoint, _referencePoint1, _referencePoint2);

            return new AlignmentResult
            {
                XOffset = relativePos.HorizontalDistance,
                YOffset = relativePos.VerticalDistance,
                IsInTolerance = Math.Abs(relativePos.VerticalDistance) < 1.0, // 示例容差
                RelativePosition = relativePos
            };
        }
    }

    public class AlignmentResult
    {
        public double XOffset { get; set; }
        public double YOffset { get; set; }
        public bool IsInTolerance { get; set; }
        public RelativePositionResult RelativePosition { get; set; }
    }
}
