#include "pch.h"
#include "newClassTry.h"


newClassTry::newClassTry()
{

	unsigned int a = 0;

}


newClassTry::~newClassTry()
{
}


// MapUtils
////////////////////////////////////////////////////////////////////////////////////////////

std::wstring newClassTry::toUtf16(const char* utf8String) {
	DWORD numCharacters = MultiByteToWideChar(CP_UTF8, 0, utf8String, -1, nullptr, 0);
	auto wideText = new std::wstring::value_type[numCharacters];
	MultiByteToWideChar(CP_UTF8, 0, utf8String, -1, wideText, numCharacters);
	std::wstring result(wideText);
	delete[] wideText;
	return result;
}

std::wstring newClassTry::UnicodeFromAscii(const std::string &asciiString) {

	int nLength = MultiByteToWideChar(CP_ACP, 0, asciiString.c_str(), -1, NULL, NULL);

	LPWSTR lpwszStr = new wchar_t[nLength];
	MultiByteToWideChar(CP_ACP, 0, asciiString.c_str(), -1, lpwszStr, nLength);

	std::wstring res = lpwszStr;
	delete[] lpwszStr;
	return res;
}

// --------------------------------------------------------------------------------------------- //

std::string newClassTry::AsciiFromUnicode(const std::wstring &unicodeString) {

	int nLen = WideCharToMultiByte(CP_ACP, 0, unicodeString.c_str(), -1, NULL, 0, NULL, NULL);
	if (nLen == 0)
	{
		return NULL;
	}
	char* pResult = new char[nLen];
	WideCharToMultiByte(CP_ACP, 0, unicodeString.c_str(), -1, pResult, nLen, NULL, NULL);
	std::string res = pResult;
	delete[] pResult;
	return res;
}

int newClassTry::ConvertGetInt(unsigned char* b, int startIndex)
{
	int ch1 = (b[3 + startIndex]) & 0xff;
	int ch2 = (b[2 + startIndex]) & 0xff;
	int ch3 = (b[1 + startIndex]) & 0xff;
	int ch4 = (b[0 + startIndex]) & 0xff;
	int r = (ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4);
	return r;
}

short newClassTry::ConvertGetShort(unsigned char * b, int startIndex)
{
	int ch1 = (b[1 + startIndex]) & 0xff;
	int ch2 = (b[0 + startIndex]) & 0xff;
	short r = (short)((ch1 << 8) + ch2);
	return r;
}

void newClassTry::ConvertMoveArray(unsigned char* src, int offsrc, unsigned char* dest, int offdest, int size)
{
	unsigned char * tBytes = new unsigned char[size];
	memcpy(tBytes, src + offsrc, size);
	memcpy(dest + offdest, tBytes, size);
	delete[] tBytes;
}

bool newClassTry::isPowOf2(int n)
{
	return true;
	//return (n & n - 1) == 0;
}

unsigned int newClassTry::getScaleLength(float curZoomer)
{
	// 支持浮点型的level
	static unsigned int lengths[] = { 32, 26, 26, 25, 20, 20, 20, 25, 32, 32, 33, 26, 26, 28, 26, 26, 26, 26, 26, 26 };
	unsigned int level = std::floor(curZoomer) - 1;
	float factor = curZoomer - level + 1;
	unsigned int width = lengths[level] * 1.5; // 对比google地图需要增加50%
	width *= factor;
	return width;
}

std::wstring newClassTry::getScaleDesc(float curZoomer)
{
	unsigned int level = std::floor(curZoomer) - 1;
	std::wstring str;
	std::wstringstream ss(std::wstringstream::in | std::wstringstream::out);
	

	static unsigned int scales[] = { 5000000, 2000000, 1000000, 500000, 200000, 100000, 50000, 30000, 20000, 10000, 5000, 2000, 1000, 500, 200, 100, 50, 25, 10, 5 };

	if (scales[level] % 1000 == 0) {
		unsigned int kmScale = scales[level] / 1000;
		ss << kmScale;
		str = ss.str();
		str += L" 公里";
		return str;
	}
	else {
		ss << scales[level];
		str = ss.str();
		str += L" 米";
		return str;
	}
}

unsigned char *  newClassTry::readFile(const std::string& filename, int& datasize)
{
	datasize = 0;

	FILE* pFile = fopen(filename.c_str(), "rb");
	fseek(pFile, 0, SEEK_END);
	datasize = ftell(pFile);

	unsigned char* szBuf = new unsigned char[datasize];
	memset(szBuf, 0, datasize);
	fseek(pFile, 0, SEEK_SET);
	int iRead = fread_s(szBuf, datasize, 1, datasize, pFile);
	fclose(pFile);

	if (iRead != datasize)
	{
		delete[] szBuf;
		szBuf = nullptr;
	}

	return szBuf;
}