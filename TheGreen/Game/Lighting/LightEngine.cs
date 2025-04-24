﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TheGreen.Game.Tiles;
using TheGreen.Game.WorldGeneration;

namespace TheGreen.Game.Lighting
{
    public class LightEngine
    {
        private (Vector3 light, Vector3 mask)[] _lightMap;
        private Point _paddedDrawBoxMin;
        private Point _paddedDrawBoxMax;
        private int _lightRange;
        private Vector3 _wallAbsorption = new Vector3(0.9f, 0.9f, 0.9f);
        private Vector3 _tileAbsorption = new Vector3(0.7f, 0.7f, 0.7f);
        private Vector3 _liquidLightAbsorption = new Vector3(0.7f, 0.8f, 0.9f);
        private Queue<(int, int, Vector3)> _dynamicLights;

        public LightEngine(GraphicsDevice graphicsDevice)
        {
            _lightRange = 38;
            _lightMap = new (Vector3, Vector3)[(TheGreen.DrawDistance.X + 2 * _lightRange) * (TheGreen.DrawDistance.Y + 2 * _lightRange)];
            _dynamicLights = new Queue<(int, int, Vector3)>();
        }
        /// <summary>
        /// Apply absorption values and default colors to light map before performing blur
        /// </summary>
        private void ClearLightMap()
        {
            Parallel.For(_paddedDrawBoxMin.X, _paddedDrawBoxMax.X, x =>
            {
                for (int y = _paddedDrawBoxMin.Y; y < _paddedDrawBoxMax.Y; y++)
                {
                    int mapIndex = (y - _paddedDrawBoxMin.Y) * (TheGreen.DrawDistance.X + 2 * _lightRange) + (x - _paddedDrawBoxMin.X);
                    if (TileDatabase.TileHasProperty(WorldGen.World.GetTileID(x, y), TileProperty.Solid))
                    {
                        _lightMap[mapIndex].light = Vector3.Zero;
                        _lightMap[mapIndex].mask = _tileAbsorption;
                    }
                    else if (WorldGen.World.GetWallID(x, y) != 0)
                    {
                        _lightMap[mapIndex].light = Vector3.Zero;
                        _lightMap[mapIndex].mask = _wallAbsorption;
                    }
                    else
                    {
                        _lightMap[mapIndex].light = new Vector3(Main.GameClock.GlobalLight / 255.0f);
                        _lightMap[mapIndex].mask = _wallAbsorption;
                    }
                    if (WorldGen.World.GetLiquid(x, y) != 0)
                    {
                        _lightMap[mapIndex].light = Vector3.Zero;
                        _lightMap[mapIndex].mask = _liquidLightAbsorption;
                    }
                    if (TileDatabase.TileHasProperty(WorldGen.World.GetTileID(x, y), TileProperty.LightEmitting))
                    {
                        _lightMap[mapIndex].light = Vector3.Max(TileDatabase.GetTileData(WorldGen.World.GetTileID(x, y)).MapColor.ToVector3(), _lightMap[mapIndex].light);
                        _lightMap[mapIndex].mask = new Vector3(1f, 1f, 1f);
                    }
                }
            });
        }
        private void ApplyDynamicLights()
        {
            while (_dynamicLights.Count > 0)
            {
                (int x, int y, Vector3 light) = _dynamicLights.Dequeue();
                if (_paddedDrawBoxMin.X <= x && x < _paddedDrawBoxMax.X && _paddedDrawBoxMin.Y <= y && y < _paddedDrawBoxMax.Y)
                {
                    int mapIndex = (y - _paddedDrawBoxMin.Y) * (TheGreen.DrawDistance.X + 2 * _lightRange) + (x - _paddedDrawBoxMin.X);
                    Vector3.Max(light, _lightMap[mapIndex].light);
                }
            }
        }
        public void CalculateLightMap()
        {
            //Perform two passes of light bluring, (possibly change this to spread left and down, then right and up for more readability
            ClearLightMap();
            ApplyDynamicLights();
            SpreadLight();
            SpreadLight();
        }
        private void SpreadLight()
        {
            int width = TheGreen.DrawDistance.X + 2 * _lightRange;
            int height = TheGreen.DrawDistance.Y + 2 * _lightRange;
            Parallel.For(0, width, x =>
            {
                SpreadLightInLine(x, x + (height - 1) * width, width);
                SpreadLightInLine(x + (height - 1) * width, x, -width);
            });

            Parallel.For(0, height, y =>
            {
                SpreadLightInLine(y * width, y * width + width - 1, 1);
                SpreadLightInLine(y * width + width - 1, y * width, -1);
            });
        }
        private void SpreadLightInLine(int startIndex, int endIndex, int stride)
        {
            Vector3 light = Vector3.Zero;
            for (int i = startIndex; i != endIndex + stride; i += stride)
            {
                light = Vector3.Max(light, _lightMap[i].light);
                if (light.X >= 0.0185f)
                {
                    _lightMap[i].light.X = light.X;
                    light.X *= _lightMap[i].mask.X;
                }
                if (light.Y >= 0.0185f)
                {
                    _lightMap[i].light.Y = light.Y;
                    light.Y *= _lightMap[i].mask.Y;
                    
                }
                if (light.Z >= 0.0185f)
                {
                    _lightMap[i].light.Z = light.Z;
                    light.Z *= _lightMap[i].mask.Z;
                }
            }
        }
        public void SetDrawBox(Point drawBoxMin, Point drawBoxMax)
        {
            _paddedDrawBoxMin = new Point(Math.Max(0, drawBoxMin.X - _lightRange), Math.Max(0, drawBoxMin.Y - _lightRange));
            _paddedDrawBoxMax = new Point(Math.Min(WorldGen.World.WorldSize.X, drawBoxMax.X + _lightRange), Math.Min(WorldGen.World.WorldSize.Y, drawBoxMax.Y + _lightRange));
        }
        public Color GetLight(int x, int y)
        {
            if (_paddedDrawBoxMin.X <= x && x < _paddedDrawBoxMax.X && _paddedDrawBoxMin.Y <= y && y < _paddedDrawBoxMax.Y)
            {
                int mapIndex = (y - _paddedDrawBoxMin.Y) * (TheGreen.DrawDistance.X + 2 * _lightRange) + (x - _paddedDrawBoxMin.X);
                return new Color(_lightMap[mapIndex].light.X, _lightMap[mapIndex].light.Y, _lightMap[mapIndex].light.Z);
            }
            return default;
        }
        /// <summary>
        /// Use to add frame-by-frame lights to the map, ensure this function is called after PreCalculations, and before Final Calculations
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void AddLight(int x, int y, Color color)
        {
            _dynamicLights.Enqueue((x, y, color.ToVector3()));
        }
    }
}
