
MAKE = make
CORE = Db4objects.Db4o
TESTS = Db4objects.Db4o.Tests
TOOLS = Db4objects.Db4o.Tools
UNIT = Db4oUnit
UNIT_EXT = Db4oUnit.Extensions
ADMIN = Db4oAdmin

LIBS = Libs/net-2.0

OUTDIR = ./bin

all: prebuild build postbuild

prebuild:
	[ -d $(OUTDIR) ] || mkdir $(OUTDIR)
	cp $(LIBS)/*.dll $(OUTDIR)

build: tests admin

postbuild:

tests: tools unit_ext
	cd $(TESTS) ; $(MAKE)

tools: core
	cd $(TOOLS) ; $(MAKE)

unit_ext: unit
	cd $(UNIT_EXT) ; $(MAKE)

unit:
	cd $(UNIT) ; $(MAKE)

core:
	cd $(CORE) ; $(MAKE)

admin: tools
	cd $(ADMIN) ; $(MAKE)

clean:
	rm -rf $(OUTDIR)
