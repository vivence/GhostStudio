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

		protected TaskReadStream readTask = null;

		public bool StartRead()
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
				stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				if (!stream.CanRead)
				{
					if (null != textMesh)
					{
						textMesh.text = string.Format("Can't read: {0}", filePath);
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
			EndRead();

			readTask = CreateTask();
			readTask.taskParam = new TaskReadStream.Param();
			readTask.taskParam.stream = stream;
			readTask.taskParam.buffer = new byte[stream.Length];
			readTask.taskParam.length = (int)stream.Length;

			if (!readTask.Operate(TaskOperation.Start))
			{
				if (null != textMesh)
				{
					textMesh.text = string.Format("Class: {0}\nCan't start", 
						readTask.GetType().ToString());
					textMesh.color = Color.white;
				}
				stream.Close();
				return false;
			}
			StartCoroutine(CheckReadTaskResult(readTask, textMesh));
			return true;
		}
		public bool EndRead()
		{
			if (null == readTask)
			{
				return false;
			}
			if (!readTask.Operate(TaskOperation.End))
			{
				return false;
			}
			readTask = null;
			return true;
		}

		#region abstract
		protected abstract TaskReadStream CreateTask();
		#endregion abstract

		#region behaviour
		IEnumerator CheckReadTaskResult(TaskReadStream task, TextMesh textMesh)
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
						task.result.readLength*100f/task.runningTaskParam.length,
						task.result.readLength, 
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
					if (task.result.readLength == task.runningTaskParam.length)
					{
						textMesh.text = string.Format("Class: {0}\nRead: Ok\nLength: {1}\nContent:\n{2}", 
							task.GetType().ToString(),
							task.result.readLength,
							Encoding.UTF8.GetString(task.runningTaskParam.buffer, task.runningTaskParam.bufferOffset, task.result.readLength));
						textMesh.color = Color.blue;
					}
					else
					{
						textMesh.text = string.Format("Class: {0}\nProgress: {1}% ({2}/{3})\nRead: Failed", 
							task.GetType().ToString(), 
							task.result.readLength*100f/task.runningTaskParam.length,
							task.result.readLength, 
							task.runningTaskParam.length);
						textMesh.color = Color.red;
					}
				}
			}

			task.runningTaskParam.stream.Close();
		}
		#endregion behaviour
	}
} // namespace Ghost.Sample
