/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

#include "Stdafx.h"

using namespace System;
using namespace System::IO;

using namespace Db4objects::Db4o::IO;

namespace Db4objects
{
namespace Db4o
{
namespace IO
{

public ref class NativeIoAdapter sealed : IoAdapter {
private:
	HANDLE m_handle;
	bool m_open;
	NativeIoAdapter(HANDLE handle);
public:
	NativeIoAdapter();
	virtual IoAdapter ^Open(String ^path, bool lock, long long initialLength) override sealed;
	virtual long long GetLength() override sealed;
	virtual int Read(array<unsigned char> ^bytes, int length) override sealed;
	virtual void BlockSeek(int address, int offset) override sealed;
	virtual void Seek(long long pos) override sealed;
	virtual void Sync() override sealed;
	virtual void Write(array<unsigned char> ^bytes, int length) override sealed;
	virtual void Copy(long long oldAddress, long long newAddress, int length) override sealed;
	virtual void BlockCopy(int oldAddress, int oldAddressOffset, int newAddress, int newAddressOffset, int length) override sealed;
	virtual void Close() override sealed;
	virtual void Delete(String ^path) override sealed;
	virtual bool Exists(String ^path) override sealed;
};

}
}
}
