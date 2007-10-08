#!/usr/bin/env booi
"""
Creates a MRefBuilder.config file configured to filter out:
	1) all the namespaces that do not appear in config/dRS-namespace-summaries.xml
	2) all the types with a <exclude /> documentation tag
"""
namespace MRefConfigGenerator

import System
import System.IO
import System.Xml
import System.Reflection
import System.Collections.Generic


def getExportedTypes(path as string):
	drsAssemblyPath=Path.Combine(path, "Db4objects.Drs.dll")
	// preload db4o assembly so the reflection runtime is happy
	Assembly.ReflectionOnlyLoadFrom(Path.Combine(path, "Db4objects.Db4o.dll"))
	return groupByNamespace(Assembly.ReflectionOnlyLoadFrom(drsAssemblyPath).GetExportedTypes())
	
def groupByNamespace(types):
	groups = Dictionary[of string, List[of Type]]()
	for item as System.Type in types:
		if item.Namespace in groups:
			groups[item.Namespace].Add(item)
		else:
			group = List[of Type]()
			group.Add(item)
			groups[item.Namespace] = group
	return groups
	
def compareTypes(t1 as Type, t2 as Type):
	result = t1.Namespace.CompareTo(t2.Namespace)
	if result != 0: return result
	return t1.Name.CompareTo(t2.Name)

def loadXmlDoc(path as string):
	doc = XmlDocument()
	doc.Load(path)
	return doc
	
def queryXmlDoc(path as string, xpath as string):
	return loadXmlDoc(path).SelectNodes(xpath)

def namespacesFromXmlSummary(path as string):
	return [nameAttr.Value
			for nameAttr as XmlAttribute
			in queryXmlDoc(path, "//namespace/@name")]
	
def appendElement(parent as XmlElement, elementName as string, attributes as Hash):
	e = parent.OwnerDocument.CreateElement(elementName)
	for item in attributes:
		e.SetAttribute(item.Key, item.Value)
	parent.AppendChild(e)	
	return e
	
def resetApiFilters(path as string):
	doc = loadXmlDoc(path)
	filters as XmlElement = doc.SelectSingleNode("//apiFilter")
	filters.RemoveAll()
	return filters
	
def getExcludedTypes(xmldocPath as string):
	return [name.Value[2:]
			for name as XmlAttribute
			in queryXmlDoc(xmldocPath, "//member[exclude]/@name")]		

if len(argv) == 2:
	 baseConfigPath, baseDistPath = argv
else:
	basePath, = argv
	baseConfigPath = Path.Combine(basePath, "config")
	baseDistPath = basePath

buildDistPath = { path  | Path.Combine(baseDistPath, path) }
buildConfigPath = { path | Path.Combine(baseConfigPath, path) }

configTemplatePath = buildConfigPath("sandcastle/MRefBuilder.config")
configPath = buildDistPath("ndoc/Output/MRefBuilder.config")
assemblyPath = buildDistPath("bin/")
namespaceSummaryPath = buildConfigPath("dRS-namespace-summaries.xml")
assemblyPathDrs = Path.Combine(assemblyPath,"Db4objects.Drs.dll")
xmldocPath = Path.ChangeExtension(assemblyPathDrs, ".xml")


try: 
	File.Copy(configTemplatePath, configPath, true)
	
	documentedNamespaces = namespacesFromXmlSummary(namespaceSummaryPath)
	excludedTypes = getExcludedTypes(xmldocPath)
	
	filters = resetApiFilters(configPath)	
	for namespaceGroup in getExportedTypes(assemblyPath):
		currentNamespace = namespaceGroup.Key
		if currentNamespace in documentedNamespaces:
			filter = appendElement(filters, "namespace", {"name": currentNamespace, "expose": "true"})
			for type as Type in namespaceGroup.Value:
				if type.FullName in excludedTypes:
					appendElement(filter, "type", { "name": type.Name, "expose": "false" })								
		else:
			appendElement(filters, "namespace", {"name": currentNamespace, "expose": "false" })		
	
	filters.OwnerDocument.Save(configPath)
	
			
	print "MRefBuilder.config successfully saved to '${configPath}'"
except x:
	print x
