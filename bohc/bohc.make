

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG_X86
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/bohc.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

BOHC_EXE_MDB_SOURCE=bin/Debug/bohc.exe.mdb
BOHC_EXE_MDB=$(BUILD_DIR)/bohc.exe.mdb
BOH_INTERNAL_H_SOURCE=boh_internal.h
BOH_INTERNAL_C_SOURCE=boh_internal.c

endif

if ENABLE_RELEASE_X86
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/bohc.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

BOHC_EXE_MDB=
BOH_INTERNAL_H_SOURCE=boh_internal.h
BOH_INTERNAL_C_SOURCE=boh_internal.c

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(BOHC_EXE_MDB) \
	$(BOH_INTERNAL_H) \
	$(BOH_INTERNAL_C)  

BINARIES = \
	$(BOHC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	boh/Exception.cs \
	generation/mangling/CMangler.cs \
	exceptions/CodeGenException.cs \
	exceptions/ParserException.cs \
	generation/ICodeGen.cs \
	generation/mangling/DebugMangler.cs \
	generation/mangling/IMangler.cs \
	Parser.cs \
	parsing/expressions/ConstructorCall.cs \
	parsing/expressions/DefaultExpressionParser.cs \
	parsing/expressions/ExprEnumerator.cs \
	parsing/expressions/Expression.cs \
	parsing/expressions/ExprType.cs \
	parsing/expressions/ExprVariable.cs \
	parsing/expressions/FunctionCall.cs \
	parsing/expressions/FunctionVarCall.cs \
	parsing/expressions/IExpressionParser.cs \
	parsing/expressions/Lambda.cs \
	parsing/expressions/Literal.cs \
	parsing/expressions/BinaryOperation.cs \
	parsing/expressions/NewDummy.cs \
	parsing/expressions/OperationType.cs \
	parsing/expressions/Operator.cs \
	parsing/expressions/ExprPackage.cs \
	parsing/expressions/RefExpression.cs \
	parsing/expressions/SuperVar.cs \
	parsing/expressions/TypeCast.cs \
	parsing/ParserTools.cs \
	parsing/statements/Body.cs \
	parsing/statements/BodyStatement.cs \
	parsing/statements/BreakStatement.cs \
	parsing/statements/CaseLabel.cs \
	parsing/statements/CatchStatement.cs \
	parsing/statements/ContinueStatement.cs \
	parsing/statements/DefaultLabel.cs \
	parsing/statements/DefaultStatementParser.cs \
	parsing/statements/DoWhileStatement.cs \
	parsing/statements/ElseStatement.cs \
	parsing/statements/EmptyStatement.cs \
	parsing/statements/ExpressionStatement.cs \
	parsing/statements/FinallyStatement.cs \
	parsing/statements/ForeachStatement.cs \
	parsing/statements/ForStatement.cs \
	parsing/statements/IfStatement.cs \
	parsing/statements/IStatementParser.cs \
	parsing/statements/ReturnStatement.cs \
	parsing/statements/Scope.cs \
	parsing/statements/Statement.cs \
	parsing/statements/SwitchLabel.cs \
	parsing/statements/SwitchStatement.cs \
	parsing/statements/ThrowStatement.cs \
	parsing/statements/TryStatement.cs \
	parsing/statements/VarDeclaration.cs \
	parsing/statements/WhileStatement.cs \
	parsing/expressions/ThisVar.cs \
	parsing/expressions/UnaryOperation.cs \
	Program.cs \
	Properties/AssemblyInfo.cs \
	typesys/Class.cs \
	typesys/CompatibleWithAllType.cs \
	typesys/Constructor.cs \
	typesys/Enum.cs \
	typesys/Enumerator.cs \
	typesys/Field.cs \
	parsing/File.cs \
	typesys/Function.cs \
	typesys/FunctionRefType.cs \
	typesys/GenericFunction.cs \
	typesys/IFunction.cs \
	typesys/IMember.cs \
	typesys/Interface.cs \
	typesys/IType.cs \
	typesys/LambdaParam.cs \
	typesys/Local.cs \
	typesys/Modifiers.cs \
	typesys/ModifierHelper.cs \
	typesys/NativeType.cs \
	typesys/NullType.cs \
	typesys/OverloadedOperator.cs \
	typesys/Package.cs \
	typesys/Parameter.cs \
	typesys/StaticConstructor.cs \
	typesys/StdType.cs \
	typesys/Struct.cs \
	typesys/Type.cs \
	typesys/GenericType.cs \
	typesys/Primitive.cs \
	typesys/Variable.cs \
	generation/c/CCodeGen.cs \
	generation/cs/CSCodeGen.cs \
	generation/mangling/CSMangler.cs \
	typesys/NativeFunction.cs \
	parsing/expressions/NativeFunctionCall.cs \
	generation/asm/IIntermediateGenerator.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	boh_internal.h \
	codegen.md \
	parsing.md \
	boh_internal.c \
	README.md \
	generation/asm \
	bohc.in 

REFERENCES =  \
	System \
	System.Core \
	System.Xml \
	System.Xml.Linq

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

BOH_INTERNAL_H = $(BUILD_DIR)/boh_internal.h
BOH_INTERNAL_C = $(BUILD_DIR)/boh_internal.c
BOHC = $(BUILD_DIR)/bohc

$(eval $(call emit-deploy-target,BOH_INTERNAL_H))
$(eval $(call emit-deploy-target,BOH_INTERNAL_C))
$(eval $(call emit-deploy-wrapper,BOHC,bohc,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
