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
	public class SampleTaskTextFileStream : MonoBehaviour 
	{
		public TaskDriver driver = null;
		public TextMesh textMesh = null;
		public string filePath = null;
		public int readPartMaxSize = 1024;

		private SyncReadStream syncReadTask = null;

		public bool StartSyncRead()
		{
			if (null == driver)
			{
				return false;
			}
			if (string.IsNullOrEmpty(filePath))
			{
				return false;
			}

			FileStream stream = null;
			try
			{
				stream = new FileStream(filePath, FileMode.Open);
				if (!stream.CanRead)
				{
					stream.Close();
					return false;
				}
			}
			catch (IOException)
			{
				if (null != stream)
				{
					stream.Close();
				}
				return false;
			}
			if (0 >= stream.Length)
			{
				stream.Close();
				return false;
			}
			EndSyncReadTask();

			syncReadTask = Factory.Create<SyncReadStream>(driver);
			syncReadTask.taskParam = new TaskReadStream.Param();
			syncReadTask.taskParam.stream = stream;
			syncReadTask.taskParam.buffer = new byte[stream.Length];
			syncReadTask.taskParam.length = (int)stream.Length;
			if (0 >= readPartMaxSize)
			{
				readPartMaxSize = 1;
			}
			syncReadTask.partLength = readPartMaxSize;

			if (!syncReadTask.Operate(TaskOperation.Start))
			{
				stream.Close();
				return false;
			}
			StartCoroutine(CheckReadTaskResult(syncReadTask, textMesh));
			return true;
		}
		public bool EndSyncReadTask()
		{
			if (null == syncReadTask)
			{
				return false;
			}
			if (!syncReadTask.Operate(TaskOperation.End))
			{
				return false;
			}
			syncReadTask = null;
			return true;
		}

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
