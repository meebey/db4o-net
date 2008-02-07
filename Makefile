
MAKE = make
CORE = Db4objects.Db4o
TESTS = Db4objects.Db4o.Tests
UNIT = Db4oUnit
UNIT_EXT = Db4oUnit.Extensions
INSTR = Db4objects.Db4o.Instrumentation
NQ = Db4objects.Db4o.NativeQueries
LINQ = Db4objects.Db4o.Linq
LINQ_TESTS = Db4objects.Db4o.Linq.Tests

LIBS = Libs/net-2.0

OUTDIR = ./bin

all: prebuild build postbuild

prebuild:
	[ -d $(OUTDIR) ] || mkdir $(OUTDIR)
	cp $(LIBS)/*.dll $(OUTDIR)

build: core nq linq tests

postbuild:

tests: unit_ext linq_tests
	cd $(TESTS) ; $(MAKE)

linq_tests:
	cd $(LINQ_TESTS) ; $(MAKE)

unit_ext: unit
	cd $(UNIT_EXT) ; $(MAKE)

unit:
	cd $(UNIT) ; $(MAKE)

instr:
	cd $(INSTR) ; $(MAKE)

nq: instr
	cd $(NQ) ; $(MAKE)

linq:
	cd $(LINQ) ; $(MAKE)

core:
	cd $(CORE) ; $(MAKE)

clean:
	rm -rf $(OUTDIR)
