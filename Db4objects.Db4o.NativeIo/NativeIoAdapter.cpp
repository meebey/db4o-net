/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

#include "Stdafx.h"
#include "NativeIoAdapter.h"

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

using namespace cli;

#define LPCWSTR_FROM_MSTR(x) ((LPCWSTR) Marshal::StringToHGlobalUni(x).ToPointer())
#define FREE_LPCWSTR(x) (Marshal::FreeHGlobal(IntPtr((void *)x)))
#define SEEK(h,p) (SetFilePointerEx(h, *reinterpret_cast<LARGE_INTEGER*>(&p), 0, FILE_BEGIN))
#define RA(adr, size, offset) (adr * size + offset)

namespace Db4objects
{
namespace Db4o
{
namespace IO
{

NativeIoAdapter::NativeIoAdapter(HANDLE handle)
{
	m_handle = handle;
	m_open = true;
}


NativeIoAdapter::NativeIoAdapter()
{
	m_handle = NULL;
	m_open = false;
}

IoAdapter ^NativeIoAdapter::Open(String ^path, bool lock, long long initialLength)
{
	LPCWSTR file = LPCWSTR_FROM_MSTR(path);
	HANDLE handle = CreateFile(
		file,
		GENERIC_READ | GENERIC_WRITE,
		lock ? 0 : FILE_SHARE_READ,
		NULL,
		OPEN_ALWAYS,
		FILE_FLAG_RANDOM_ACCESS,
		NULL);

	FREE_LPCWSTR(file);

	if (handle == INVALID_HANDLE_VALUE)
		throw gcnew IOException(String::Concat("Failed to get an handle to ", path));

	NativeIoAdapter ^nio = gcnew NativeIoAdapter(handle);

	if (initialLength > 0)
	{
		nio->Seek(initialLength - 1);
		nio->Write (gcnew array<unsigned char> {0}, 1);
	}

	return nio;
}

long long NativeIoAdapter::GetLength()
{
	if (!m_open) return 0;
	long long length;
	GetFileSizeEx(m_handle, reinterpret_cast<LARGE_INTEGER *>(&length));
	return length;
}

int NativeIoAdapter::Read(array<unsigned char> ^bytes, int length)
{
	if (!m_open) return 0;
	DWORD read;
	pin_ptr<unsigned char> pb = &bytes[0];
	ReadFile(m_handle, pb, length, &read, NULL);
	return read;
}

void NativeIoAdapter::BlockSeek(int address, int offset)
{
	if (!m_open) return;
	int size = BlockSize();
	Seek(RA(address, size, offset));
}

void NativeIoAdapter::Seek(long long position)
{
	if (!m_open) return;
	SEEK(m_handle, position);
}

void NativeIoAdapter::Sync()
{
	if (!m_open) return;
	FlushFileBuffers(m_handle);
}

void NativeIoAdapter::Write(array<unsigned char> ^bytes, int length)
{
	if (!m_open) return;
	DWORD write;
	pin_ptr<unsigned char> pb = &bytes[0];
	WriteFile(m_handle, pb, length, &write, NULL);
}

void NativeIoAdapter::BlockCopy(int oldAddress, int oldAddressOffset, int newAddress, int newAddressOffset, int length)
{
	if (!m_open) return;
	int size = BlockSize();
	Copy(RA(oldAddress, size, oldAddressOffset), RA(newAddress, size, newAddressOffset), length);
}

void NativeIoAdapter::Copy(long long oldAddress, long long newAddress, int length)
{
	if (!m_open) return;
	DWORD read, write;
	HLOCAL buffer = LocalAlloc(LMEM_FIXED, length);
	if (NULL == buffer) return;

	SEEK(m_handle, oldAddress);
	ReadFile(m_handle, buffer, length, &read, NULL);
	if (read == length)
	{
		SEEK(m_handle, newAddress);
		WriteFile(m_handle, buffer, length, &write, NULL);
	}

	LocalFree(buffer);
}

void NativeIoAdapter::Close()
{
	if (!m_open) return;
	CloseHandle(m_handle);
}

void NativeIoAdapter::Delete(String ^path)
{
	LPCWSTR file = LPCWSTR_FROM_MSTR(path);
	DeleteFile(file);
	FREE_LPCWSTR(file);
}

bool NativeIoAdapter::Exists(String ^path)
{
	return File::Exists(path);
}

}
}
}
