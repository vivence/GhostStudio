using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Ghost.Task;
using Ghost.Task.IO;
using Ghost.Extension;
using Ghost.Utility;

namespace Ghost.Sample
{
	public abstract class SampleTaskTextFileStream : MonoBehaviour 
	{
		public TaskDriver driver = null;
		public TextMesh textMesh = null;
		public string filePath = null;
		public TaskStream.Access access = TaskStream.Access.Read;
		public string writeData = null;

		protected TaskStream accessTask = null;

		public bool StartAccess()
		{
			if (null == driver)
			{
				if (null != textMesh)
				{
					textMesh.text = string.Format("No driver");
					textMesh.color = Color.red;
				}
				return false;
			}
			if (string.IsNullOrEmpty(filePath))
			{
				if (null != textMesh)
				{
					textMesh.text = string.Format("No file path");
					textMesh.color = Color.red;
				}
				return false;
			}

			FileStream stream = null;
			try
			{
				var path = Path.Combine(Application.streamingAssetsPath, filePath);
				bool canAccess = false;
				switch (access)
				{
				case TaskStream.Access.Read:
					stream = new FileStream(path, FileMode.Open, FileAccess.Read);
					canAccess = stream.CanRead;
					break;
				case TaskStream.Access.Write:
					if (!Directory.Exists(Path.GetDirectoryName(path)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(path));
					}
					stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
					canAccess = stream.CanWrite;
					break;
				}

				if (!canAccess)
				{
					if (null != textMesh)
					{
						textMesh.text = string.Format("Can't {0}: {1}", access.ToString(), filePath);
						textMesh.color = Color.red;
					}
					stream.Close();
					return false;
				}
			}
			catch (IOException e)
			{
				if (null != textMesh)
				{
					textMesh.text = string.Format("Open file Failed: {0}", e.Message);
					textMesh.color = Color.red;
				}
				if (null != stream)
				{
					stream.Close();
				}
				return false;
			}

			byte[] buffer = null;
			switch (access)
			{
			case TaskStream.Access.Read:
				if (0 >= stream.Length)
				{
					if (null != textMesh)
					{
						textMesh.text = string.Format("File is empty: {0}", filePath);
						textMesh.color = Color.red;
					}
					stream.Close();
					return false;
				}
				buffer = new byte[stream.Length];
				break;
			case TaskStream.Access.Write:
				if (string.IsNullOrEmpty(writeData))
				{
					if (null != textMesh)
					{
						textMesh.text = string.Format("Write data is empty");
						textMesh.color = Color.red;
					}
					stream.Close();
					return false;
				}
				buffer = Encoding.UTF8.GetBytes(writeData);
				break;
			}

			EndAccess();

			accessTask = CreateTask();
			accessTask.taskParam = new TaskStream.Param();
			accessTask.taskParam.access = access;
			accessTask.taskParam.stream = stream;
			accessTask.taskParam.buffer = buffer;
			accessTask.taskParam.length = buffer.Length;

			if (!accessTask.Operate(TaskOperation.Start))
			{
				if (null != textMesh)
				{
					textMesh.text = string.Format("Class: {0}\nCan't start", 
						accessTask.GetType().ToString());
					textMesh.color = Color.red;
				}
				stream.Close();
				return false;
			}
			StartCoroutine(CheckReadTaskResult(accessTask, textMesh));
			return true;
		}
		public bool EndAccess()
		{
			if (null == accessTask)
			{
				return false;
			}
			if (!accessTask.Operate(TaskOperation.End))
			{
				return false;
			}
			accessTask = null;
			return true;
		}

		#region abstract
		protected abstract TaskStream CreateTask();
		#endregion abstract

		#region behaviour
		IEnumerator CheckReadTaskResult(TaskStream task, TextMesh textMesh)
		{
			while (TaskState.Pending == task.currentState)
			{
				if (null != textMesh)
				{
					textMesh.text = string.Format("Class: {0}\nPending...", 
						task.GetType().ToString());
					textMesh.color = Color.white;
				}
				yield return null;
			}
			while (TaskState.Running == task.currentState)
			{
				if (null != textMesh)
				{
					textMesh.text = string.Format("Class: {0}\nProgress: {1}% ({2}/{3})", 
						task.GetType().ToString(), 
						task.result.completedLength*100f/task.runningTaskParam.length,
						task.result.completedLength, 
						task.runningTaskParam.length);
					textMesh.color = Color.green;
				}
				yield return null;
			}
			if (null != textMesh)
			{
				if (null != task.result.exception)
				{
					textMesh.text = string.Format("Class: {0}\nException: {1}", 
						task.GetType().ToString(), task.result.exception.Message);
					textMesh.color = Color.red;
				}
				else
				{
					if (task.result.completedLength == task.runningTaskParam.length)
					{
						textMesh.text = string.Format("Class: {0}\n{1}: Ok\nLength: {2}\nContent:\n{3}", 
							task.GetType().ToString(),
							access.ToString(),
							task.result.completedLength,
							Encoding.UTF8.GetString(task.runningTaskParam.buffer, task.runningTaskParam.bufferOffset, task.result.completedLength));
						textMesh.color = Color.blue;
					}
					else
					{
						textMesh.text = string.Format("Class: {0}\nProgress: {1}% ({2}/{3})\n{4}: Failed", 
							task.GetType().ToString(), 
							task.result.completedLength*100f/task.runningTaskParam.length,
							task.result.completedLength, 
							task.runningTaskParam.length,
							access.ToString());
						textMesh.color = Color.red;
					}
				}
			}

			task.runningTaskParam.stream.Close();
		}
		#endregion behaviour
	}
} // namespace Ghost.Sample
