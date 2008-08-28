/*
 * Created by SharpDevelop.
 * User: rodrigob
 * Date: 11/1/2004
 * Time: 3:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;

namespace Db4objects.Db4o.Tutorial
{
	/// <summary>
	/// Description of TutorialOutlineView.
	/// </summary>
	public class TutorialOutlineView : DockContent
	{
		MainForm _main;
		TreeView _tree;
		
		public TutorialOutlineView(MainForm main)
		{
			_main = main;
			
			this.Text = "Tutorial Outline";
			this.DockableAreas = (DockAreas.Float |
			                      DockAreas.DockLeft |
			                      DockAreas.DockRight);
			
			this.ClientSize = new System.Drawing.Size(295, 347);
			this.DockPadding.Bottom = 2;
			this.DockPadding.Top = 2;
			this.ShowHint = DockState.DockLeft;
			this.CloseButton = false;
			
			TutorialOutlineViewControl control = new TutorialOutlineViewControl();			
			control.Dock = DockStyle.Fill;			
			_tree = control.TreeView;	
			_tree.AfterSelect += new TreeViewEventHandler(_tree_AfterSelect);
			this.Controls.Add(control);
		}
		
		private void _tree_AfterSelect(object sender, TreeViewEventArgs args)
		{
			_main.NavigateTutorial((string)args.Node.Tag);
		}
		
		override protected void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			LoadTutorialOutline();
		}
		
		delegate void LoadFirstTopicFunction();
		
		void LoadTutorialOutline()
		{			
			TreeNode current = new TreeNode();
			TreeNode root = current;
			TreeNode currentParent = null;			
			Stack nodes = new Stack();			

			string path = _main.GetTutorialFilePath("outline.html");
			if (!File.Exists(path))
			{
				return;
			}
			
			XmlTextReader reader = new XmlTextReader(path);
			while (reader.Read())
			{
				string name = reader.Name;				
				switch (name)
				{
					case "li":
						{	
							reader.ReadStartElement("li");							
							
							string href = reader.GetAttribute("href");
							
							reader.ReadStartElement("a");
							string description = reader.ReadString();
							
							current = new TreeNode(description);
							current.Tag = href;
							
							currentParent.Nodes.Add(current);
							
							reader.ReadEndElement();
							reader.ReadEndElement();
							
							break;
						}
						
					case "ul":
						{	
							if (reader.IsStartElement())
							{
								nodes.Push(currentParent);
								currentParent = current;
							}
							else
							{
								currentParent = (TreeNode)nodes.Pop();
							}
							break;
						}
				}
			}
			
			foreach (TreeNode node in root.Nodes)
			{
				_tree.Nodes.Add(node);
			}
			_tree.ExpandAll();
			
			BeginInvoke(new LoadFirstTopicFunction(LoadFirstTopic));
		}
		
		void LoadFirstTopic()
		{
			_tree.SelectedNode = _tree.Nodes[0];
		}
		
		string LoadFile(string fname)
		{
			using (TextReader reader = File.OpenText(fname))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
