﻿using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace WindowsFormsCatamaran
{
	// Параметризованный класс для хранения набора объектов от интерфейса ITransport
	public class Port<T> : IEnumerator<T>, IEnumerable<T> where T : class, ITransport
	{
		private readonly List<T> _places; // Список объектов, которые храним
		private readonly int _maxCount; // Максимальное количество мест в гавани
		private readonly int pictureWidth; // Ширина окна отрисовки
		private readonly int pictureHeight; // Высота окна отрисовки		
		private readonly int _placeSizeWidth = 220; // Ширина парковочного места
		private readonly int _placeSizeHeight = 90; // Высота парковочного места
		private int _currentIndex;
		public T Current => _places[_currentIndex];
		object IEnumerator.Current => _places[_currentIndex];

		// Конструктор
		public Port(int picWidth, int picHeight)
		{
			int width = picWidth / _placeSizeWidth;
			int height = picHeight / _placeSizeHeight;
			_maxCount = width * height;
			pictureWidth = picWidth;
			pictureHeight = picHeight;
			_places = new List<T>();
			_currentIndex = -1;
		}

		// Перегрузка оператора сложения
		public static int operator +(Port<T> p, T boat)
		{
			if (p._places.Count >= p._maxCount)
			{
				throw new PortOverflowException();
			}
			if (p._places.Contains(boat)) 
			{ 
				throw new PortAlreadyHaveException(); 
			}
			p._places.Add(boat);
			return p._places.Count - 1;
		}

		// Перегрузка оператора вычитания
		public static T operator -(Port<T> p, int index)
		{
			if (index <= -1 || index >= p._places.Count)
			{
				throw new PortNotFoundException(index);
			}
			T t = p._places[index];
			p._places.RemoveAt(index);
			return t;
		}

		// Метод отрисовки гавани
		public void Draw(Graphics g)
		{
			DrawMarking(g);
			for (int i = 0; i < _places.Count; ++i) 
			{ 
				_places[i].SetPosition(i % (pictureWidth / _placeSizeWidth) * _placeSizeWidth + 5, i / (pictureWidth / _placeSizeWidth) * _placeSizeHeight + 5, pictureWidth, pictureHeight);
				_places[i].DrawTransport(g); 
			}
		}

		// Метод отрисовки разметки парковочных мест
		private void DrawMarking(Graphics g)
		{
			Pen pen = new Pen(Color.Black, 3);
			Brush water = new SolidBrush(Color.LightBlue);
			g.FillRectangle(water, 0, 0, pictureWidth, pictureHeight);
			for (int i = 0; i < pictureWidth / _placeSizeWidth; i++)
			{
				for (int j = 0; j < pictureHeight / _placeSizeHeight + 1; ++j)
				{
					g.DrawLine(pen, i * _placeSizeWidth, j * _placeSizeHeight, i * _placeSizeWidth + _placeSizeWidth / 2, j * _placeSizeHeight);
				}
				g.DrawLine(pen, i * _placeSizeWidth, 0, i * _placeSizeWidth, (pictureHeight / _placeSizeHeight) * _placeSizeHeight);
			}
		}

		// Функция получения элементы из списка
		public T GetNext(int index)
		{
			if (index < 0 || index >= _places.Count) 
			{ 
				return null; 
			}
			return _places[index];
		}

		// Сортировка лодок в гавани
		public void Sort() => _places.Sort((IComparer<T>)new BoatComparer()); 

		// Метод интерфейса IEnumerator, вызываемый при удалении объекта
		public void Dispose()         
		{         
		} 

		// Метод интерфейса IEnumerator для перехода к следующему элементу или началу коллекции
		public bool MoveNext()         
		{
			if (_currentIndex < _places.Count - 1)
			{
				_currentIndex++;
				return true;
			}
			return false;
		} 

		// Метод интерфейса IEnumerator для сброса и возврата к началу коллекции
		public void Reset()         
		{             
			_currentIndex = -1;         
		} 

		// Метод интерфейса IEnumerable
		public IEnumerator<T> GetEnumerator()         
		{             
			return this;         
		} 

		// Метод интерфейса IEnumerable
		IEnumerator IEnumerable.GetEnumerator()         
		{             
			return this;         
		}
	}
}
