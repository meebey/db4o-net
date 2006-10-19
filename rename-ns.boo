import Useful.IO from Boo.Lang.Useful
import System.IO
import System.Text.RegularExpressions

class Application:

	def process(fname as string):
		original = TextFile.ReadFile(fname)
		contents = replace(original)
		if original != contents:
			print fname
			TextFile.WriteFile(fname, contents)
		
	def replace(contents as string):
		lines = /\n/.Split(contents)
		for line in lines:
			if line.StartsWith("using") or line.StartsWith("namespace"):
				continue
			for interfaceName in interfaces:			
				line = regex("\\b${interfaceName}\\b").Replace(line) do (match as Match):
					name = match.Groups[0].ToString()
					return "I${name}"
		return join(lines, "\n")
	
	interfaces = (
		"Db4oCollection",
		"Db4oCollections",
		"ObjectContainer", 
		"ExtObjectContainer",
		"DiagnosticListener",
		"ReflectArray",
		"ReflectClass",
		"ReflectConstructor",
		"ReflectField",
		"ReflectMethod",
		"Reflector",
		"Db4oList",
		"Db4oTypeImpl",
		"Evaluation",
		"ObjectTranslator",
		"ObjectConstructor",
		"Db4oMap",
		"TransactionListener",
		"ObjectSet",
		"QueryResult",
		"Configuration",
		"Candidate",
		"ExtObjectSet",
		"QueryComparator",
		"Closure4",
		"Visitor4",
		"Db4oEnhancedFilter",
	)
	
	def run():
		for fname as string in listFiles("../db4o.net/Db4objects.Db4o/native"):
			if ".svn" in fname: continue
			if not fname.EndsWith(".cs"): continue
			process(fname)
			
Application().run()