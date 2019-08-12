using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	
	public class ObstacleManager: MonoBehaviour
	{
		//[SerializeField]
		private List<ObstacleDescription>[] _lines;	// todo: use object pool

		[SerializeField]
		private Obstacle[] _obstaclePrefabs;

		[SerializeField]
		private Vector3[] _sizeVariation;

		[SerializeField]
		private ObstacleColor[] _colorVariation;	// todo: editor extension
		[SerializeField]
		private Material[] _materialVariation;

		[SerializeField]
		private int _lineCount = 4;

		[SerializeField]
		private int _lineStep = 4;

		[SerializeField]
		private float _initialCreationDistance = 2.5f;

		[SerializeField]
		private float _zStart = 18;


		private FigureVariation[] _differentFigures;
		private float _distanceToCreateNew = 0;
		private float _distanceFromLastObstacle = 0;
		private float _xRange = 5;
		private float _yPosition = 0.5f;
		private float _farGenerationRange = 25;
		private float _xStep = 0.3f;
		public bool IsActive = true;
		private GameManager _gameManager;
		private ObstacleColor _playerColor;
		private int _playerColorIndex;

		private void Start()
		{
			_gameManager = GameManager.Instance;
			_playerColor = _gameManager.GetPlayerColor();
			_playerColorIndex = Array.IndexOf(_colorVariation, _playerColor);

			var differentFiguresVariation = _obstaclePrefabs.Length * _sizeVariation.Length;
			_differentFigures = new FigureVariation[differentFiguresVariation];

			for (int i = 0, index = 0; i < _obstaclePrefabs.Length; i++)
			{
				for (int j = 0; j < _sizeVariation.Length; j++, index++)
				{
					_differentFigures[index] = new FigureVariation(_obstaclePrefabs[i], _sizeVariation[j]);
				}
			}

			_lines = new List<ObstacleDescription>[_lineCount];
			var zPosition = _zStart;
			for (var i = 0; i < _lineCount; i++)
			{
				_lines[i] = new List<ObstacleDescription>(15);
				var lastFigureBorder = -_xRange;
				while (lastFigureBorder < _xRange)
				{
					var currentVariation = _differentFigures[UnityEngine.Random.Range(0, _differentFigures.Length)];
					var instance = Instantiate<Obstacle>(currentVariation.Prefab, transform);
					var colorIndex = UnityEngine.Random.Range(0, _colorVariation.Length);
					instance.Init(_materialVariation[colorIndex], _colorVariation[colorIndex], currentVariation.Size);
					var figureXSize = instance.GetXSize();
					var newXPosition = _xStep + lastFigureBorder + figureXSize / 2;
					lastFigureBorder = newXPosition + figureXSize / 2;
					if (lastFigureBorder > _xRange)
					{
						GameObject.Destroy(instance.gameObject);
						break;
					}
					var position = new Vector3(newXPosition, instance.GetYSize()/2, zPosition);
					instance.SetPosition(position);
					_lines[i].Add(new ObstacleDescription(instance, colorIndex, position, figureXSize));
				}
				zPosition += _lineStep;


				// change color to ensure there are exists one way to pass
				int currentGroupIndex = 0;
				int previousColorIndex = _lines[i][0].ColorIndex;
				List<List<int>> matchIndeces = new List<List<int>>();

				for (int j = 0; j < _lines[i].Count; j++)
				{
					if (j == 0)
					{
						if (_lines[i][j].ColorIndex == _playerColorIndex)
						{
							matchIndeces.Add(new List<int>());
							matchIndeces[currentGroupIndex].Add(j);
						}
						continue;
					}
					if (_lines[i][j].ColorIndex == _playerColorIndex)
					{
						if (previousColorIndex != _playerColorIndex)
						{
							matchIndeces.Add(new List<int>());
						}
						matchIndeces[currentGroupIndex].Add(j);
					}
					else
					{
						if (previousColorIndex == _playerColorIndex)
						{
							currentGroupIndex++;
						}
					}
					previousColorIndex = _lines[i][j].ColorIndex;
				}


				var playerWidth = _gameManager.GetPlayerWidth();
				if (matchIndeces.Any())
				{
					bool exitGroupFound = false;
					var nonPlayerColors = _colorVariation.Where((w, index) => index != _playerColorIndex).ToArray();
					var nonPlayerMaterials = _materialVariation.Where((w, index) => index != _playerColorIndex).ToArray();
					for (int j = 0; j < matchIndeces.Count; j++)
					{
						var exitWidth = GroupWidth(matchIndeces[j], _lines[i]);

						if (playerWidth <= exitWidth)
						{
							if (exitGroupFound) // replace other exits
							{
								for (int k = 0; k < matchIndeces[j].Count; k++)
								{
									var newColorIndex = UnityEngine.Random.Range(0, nonPlayerColors.Length);
									_lines[i][matchIndeces[j][k]].Instance.Init(nonPlayerMaterials[newColorIndex], nonPlayerColors[newColorIndex]);
									_lines[i][matchIndeces[j][k]].ColorIndex = newColorIndex;
								}
							}
							else
							{
								exitGroupFound = true;
								Debug.Log($"<color=green>For line {i} width is {exitWidth}</color>");
							}
						}
					}

					if (!exitGroupFound)
					{
						List<int> group = matchIndeces.Last();
						float exitWidth = 0;
						while (exitWidth < playerWidth)
						{
							int firstIndex = group[0];
							int lastIndex = group[group.Count - 1];
							if (lastIndex < (_lines[i].Count - 1)) // group is not in the end of set
							{
								_lines[i][++lastIndex].Instance.Init(_materialVariation[_playerColorIndex], _colorVariation[_playerColorIndex]);
								Debug.Log($"<color=yellow>Change color in line {i} for index {lastIndex}</color>");
								group.Add(lastIndex);
								exitWidth = GroupWidth(group, _lines[i]);
							} else if (firstIndex > 0) // group is not in the begin of set
							{
								_lines[i][--firstIndex].Instance.Init(_materialVariation[_playerColorIndex], _colorVariation[_playerColorIndex]);
								Debug.Log($"<color=yellow>Change color in line {i} for index {firstIndex}</color>");
								var newGroup = new List<int>(group.Count + 1);
								newGroup.Add(firstIndex);
								newGroup.AddRange(group);
								group = newGroup;
								exitWidth = GroupWidth(group, _lines[i]);
							}
						}
						Debug.Log($"<color=green>For line {i} width is {exitWidth}</color>");
					}
				}
				else	// no matching groups (no player color in line at all)
				{
					float exitWidth = 0;
					int index = 0;
					var group = new List<int>();
					while (exitWidth < playerWidth)
					{
						_lines[i][index].Instance.Init(_materialVariation[_playerColorIndex], _colorVariation[_playerColorIndex]);
						Debug.Log($"<color=yellow>Change color in line {i} for index {index}</color>");
						group.Add(index);
						exitWidth = GroupWidth(group, _lines[i]);
						index++;
					}
					Debug.Log($"<color=green>For line {i} width is {exitWidth}</color>");
				}
			}
		}

		private float GroupWidth(List<int> indexes, List<ObstacleDescription> line)
		{
			float width = 0f;
			for (int k = 0; k < indexes.Count; k++)
			{
				width += line[indexes[k]].Width;
			}
			return width;
		}


		private class ObstacleDescription
		{
			public Obstacle Instance;
			public int ColorIndex;
			public Vector3 Position;
			public float Width;

			public ObstacleDescription(Obstacle instance, int colorIndex, Vector3 position, float width)
			{
				Instance = instance;
				ColorIndex = colorIndex;
				Position = position;
				Width = width;
				Debug.Log($"<color=blue>Item info:</color> color={colorIndex} width={width}");
			}
		}

		private class FigureVariation
		{
			public Obstacle Prefab;
			public Vector3 Size;

			public FigureVariation(Obstacle prefab, Vector3 size)
			{
				Prefab = prefab;
				Size = size;
			}
		}
	}
}
