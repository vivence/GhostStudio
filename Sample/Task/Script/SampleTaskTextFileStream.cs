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

		public int fileBufferSize = 1024;

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
					stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, fileBufferSize, fileAsync);
					canAccess = stream.CanRead;
					break;
				case TaskStream.Access.Write:
					if (!Directory.Exists(Path.GetDirectoryName(path)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(path));
					}
					stream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.None, fileBufferSize, fileAsync);
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

			accessTask.stateChangedListener += OnTaskStateChanged;
			accessTask.progressChangedListener += OnTaskProgressChanged;

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
			accessTask.stateChangedListener -= OnTaskStateChanged;
			accessTask.progressChangedListener -= OnTaskProgressChanged;
			accessTask = null;
			return true;
		}

		#region virtual
		protected virtual bool fileAsync
		{
			get
			{
				return false;
			}
		}
		#endregion virtual

		#region abstract
		protected abstract TaskStream CreateTask();
		#endregion abstract

		#region event listener
		private void OnTaskStateChanged(Entity source, TaskState oldState, TaskState newState)
		{
			var task = source as TaskStream;
			switch (newState)
			{
			case TaskState.Pending:
				if (null != textMesh)
				{
					textMesh.text = string.Format("Class: {0}\nPending...", 
						task.GetType().ToString());
					textMesh.color = Color.white;
				}
				break;
			case TaskState.Running:
				if (null != textMesh)
				{
					textMesh.text = string.Format("Class: {0}\nRunning...", 
						task.GetType().ToString());
					textMesh.color = Color.green;
				}
				break;
			case TaskState.Idle:
				if (TaskState.Running == oldState && null != textMesh)
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
				if (null != task.runningTaskParam && null != task.runningTaskParam.stream)
				{
					task.runningTaskParam.stream.Close();
				}
				break;
			}
		}
		private void OnTaskProgressChanged(Entity source, float oldProgress, float newProgress)
		{
			if (null != textMesh)
			{
				var task = source as TaskStream;
				textMesh.text = string.Format("Class: {0}\nProgress: {1}% ({2}/{3})", 
					task.GetType().ToString(), 
					task.result.completedLength*100f/task.runningTaskParam.length,
					task.result.completedLength, 
					task.runningTaskParam.length);
				textMesh.color = Color.green;
			}
		}
		#endregion event listener

		#region behaviour
		void OnDestroy()
		{
			if (null != accessTask && null != accessTask.runningTaskParam.stream)
			{
				var stream = accessTask.runningTaskParam.stream;
				EndAccess();
				stream.Close();
			}
		}
		#endregion behaviour
	}
} // namespace Ghost.Sample
