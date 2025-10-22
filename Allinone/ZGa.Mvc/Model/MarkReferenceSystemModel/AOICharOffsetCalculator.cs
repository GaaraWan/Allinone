using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ZGa.Mvc.Model.MarkReferenceSystemModel
{

    // AOI字符偏移计算结果
    public class AOICharOffsetResult
    {
        public double DeltaX { get; set; }        // X方向偏移量
        public double DeltaY { get; set; }        // Y方向偏移量
        public double Rotation { get; set; }      // 旋转角度(度)
        public bool IsValid { get; set; }         // 计算结果是否有效
        public string ErrorMessage { get; set; }  // 错误信息
    }
    public class AOICharOffsetCalculator
    {
        // 理论坐标（设计坐标）
        public PointF TheoreticalMarkPoint { get; set; }

        // 实际测量坐标
        public PointF ActualMarkPoint { get; set; }

        // 字符的理论位置
        public PointF TheoreticalCharPosition { get; set; }

        public AOICharOffsetCalculator()
        {
        }

        /// <summary>
        /// 计算字符的偏移量（基于单个Mark点）
        /// </summary>
        /// <param name="theoreticalMark">理论Mark点坐标</param>
        /// <param name="actualMark">实际Mark点坐标</param>
        /// <param name="theoreticalChar">理论字符坐标</param>
        public AOICharOffsetCalculator(PointF theoreticalMark, PointF actualMark, PointF theoreticalChar)
        {
            TheoreticalMarkPoint = theoreticalMark;
            ActualMarkPoint = actualMark;
            TheoreticalCharPosition = theoreticalChar;
        }

        /// <summary>
        /// 计算字符偏移（平移，无旋转）
        /// </summary>
        public AOICharOffsetResult CalculateOffset(PointF actualChar)
        {
            var result = new AOICharOffsetResult();

            try
            {
                // 计算Mark点的整体偏移
                double deltaX = ActualMarkPoint.X - TheoreticalMarkPoint.X;
                double deltaY = ActualMarkPoint.Y - TheoreticalMarkPoint.Y;

                // 字符的预期实际位置 = 理论位置 + 整体偏移
                double expectedCharX = TheoreticalCharPosition.X + deltaX;
                double expectedCharY = TheoreticalCharPosition.Y + deltaY;

                result.DeltaX = actualChar.X - expectedCharX;
                result.DeltaY = actualChar.Y - expectedCharY;
                result.Rotation = 0;  // 单Mark点无法计算旋转
                result.IsValid = true;

                Console.WriteLine($"字符偏移计算结果:");
                Console.WriteLine($"X方向偏移: {deltaX:F3} mm");
                Console.WriteLine($"Y方向偏移: {deltaY:F3} mm");
                Console.WriteLine($"字符预期位置: ({expectedCharX:F3}, {expectedCharY:F3})");
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"计算失败: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 计算字符偏移（带手动旋转角度）
        /// </summary>
        /// <param name="rotationAngle">手动指定的旋转角度（度）</param>
        public AOICharOffsetResult CalculateOffsetWithRotation(double rotationAngle)
        {
            var result = new AOICharOffsetResult();

            try
            {
                // 基本平移偏移
                double deltaX = ActualMarkPoint.X - TheoreticalMarkPoint.X;
                double deltaY = ActualMarkPoint.Y - TheoreticalMarkPoint.Y;

                // 将旋转角度转换为弧度
                double angleRad = rotationAngle * Math.PI / 180.0;

                // 计算相对于Mark点的字符向量
                double charRelativeX = TheoreticalCharPosition.X - TheoreticalMarkPoint.X;
                double charRelativeY = TheoreticalCharPosition.Y - TheoreticalMarkPoint.Y;

                // 应用旋转矩阵
                double rotatedX = charRelativeX * Math.Cos(angleRad) - charRelativeY * Math.Sin(angleRad);
                double rotatedY = charRelativeX * Math.Sin(angleRad) + charRelativeY * Math.Cos(angleRad);

                // 计算字符的预期实际位置
                double expectedCharX = TheoreticalMarkPoint.X + rotatedX + deltaX;
                double expectedCharY = TheoreticalMarkPoint.Y + rotatedY + deltaY;

                result.DeltaX = expectedCharX - TheoreticalCharPosition.X;
                result.DeltaY = expectedCharY - TheoreticalCharPosition.Y;
                result.Rotation = rotationAngle;
                result.IsValid = true;

                Console.WriteLine($"带旋转的字符偏移计算结果:");
                Console.WriteLine($"X方向偏移: {result.DeltaX:F3} mm");
                Console.WriteLine($"Y方向偏移: {result.DeltaY:F3} mm");
                Console.WriteLine($"旋转角度: {rotationAngle:F3}°");
                Console.WriteLine($"字符预期位置: ({expectedCharX:F3}, {expectedCharY:F3})");
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"计算失败: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 验证偏移是否在允许范围内
        /// </summary>
        /// <param name="result">偏移计算结果</param>
        /// <param name="toleranceX">X方向容差</param>
        /// <param name="toleranceY">Y方向容差</param>
        public bool ValidateOffset(AOICharOffsetResult result, double toleranceX, double toleranceY)
        {
            if (!result.IsValid)
                return false;

            return Math.Abs(result.DeltaX) <= toleranceX &&
                   Math.Abs(result.DeltaY) <= toleranceY;
        }
    }
}
