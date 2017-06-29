using System;
using System.Collections.Generic;
using System.Text;

namespace AppMetrics.Logic.Utils
{
    public class TripleDes
    {
    public class Des3Context{
        public int mode;                   /*!<  encrypt/decrypt   */
        public uint[] sk = new uint[96];       /*!<  3DES subkeys      */        
    }
    private class Aux{
        public uint value;
    }
    Des3Context context;
    
    public static bool DES_ENCRYPT    =   true;
    public static bool DES_DECRYPT    =   false;
    /*
     * Expanded DES S-boxes
     */
    static uint[] SB1 =
    {
        0x01010400, 0x00000000, 0x00010000, 0x01010404,
        0x01010004, 0x00010404, 0x00000004, 0x00010000,
        0x00000400, 0x01010400, 0x01010404, 0x00000400,
        0x01000404, 0x01010004, 0x01000000, 0x00000004,
        0x00000404, 0x01000400, 0x01000400, 0x00010400,
        0x00010400, 0x01010000, 0x01010000, 0x01000404,
        0x00010004, 0x01000004, 0x01000004, 0x00010004,
        0x00000000, 0x00000404, 0x00010404, 0x01000000,
        0x00010000, 0x01010404, 0x00000004, 0x01010000,
        0x01010400, 0x01000000, 0x01000000, 0x00000400,
        0x01010004, 0x00010000, 0x00010400, 0x01000004,
        0x00000400, 0x00000004, 0x01000404, 0x00010404,
        0x01010404, 0x00010004, 0x01010000, 0x01000404,
        0x01000004, 0x00000404, 0x00010404, 0x01010400,
        0x00000404, 0x01000400, 0x01000400, 0x00000000,
        0x00010004, 0x00010400, 0x00000000, 0x01010004
    };

    static uint[] SB2 =
    {
        0x80108020, 0x80008000, 0x00008000, 0x00108020,
        0x00100000, 0x00000020, 0x80100020, 0x80008020,
        0x80000020, 0x80108020, 0x80108000, 0x80000000,
        0x80008000, 0x00100000, 0x00000020, 0x80100020,
        0x00108000, 0x00100020, 0x80008020, 0x00000000,
        0x80000000, 0x00008000, 0x00108020, 0x80100000,
        0x00100020, 0x80000020, 0x00000000, 0x00108000,
        0x00008020, 0x80108000, 0x80100000, 0x00008020,
        0x00000000, 0x00108020, 0x80100020, 0x00100000,
        0x80008020, 0x80100000, 0x80108000, 0x00008000,
        0x80100000, 0x80008000, 0x00000020, 0x80108020,
        0x00108020, 0x00000020, 0x00008000, 0x80000000,
        0x00008020, 0x80108000, 0x00100000, 0x80000020,
        0x00100020, 0x80008020, 0x80000020, 0x00100020,
        0x00108000, 0x00000000, 0x80008000, 0x00008020,
        0x80000000, 0x80100020, 0x80108020, 0x00108000
    };

    static uint[] SB3 =
    {
        0x00000208, 0x08020200, 0x00000000, 0x08020008,
        0x08000200, 0x00000000, 0x00020208, 0x08000200,
        0x00020008, 0x08000008, 0x08000008, 0x00020000,
        0x08020208, 0x00020008, 0x08020000, 0x00000208,
        0x08000000, 0x00000008, 0x08020200, 0x00000200,
        0x00020200, 0x08020000, 0x08020008, 0x00020208,
        0x08000208, 0x00020200, 0x00020000, 0x08000208,
        0x00000008, 0x08020208, 0x00000200, 0x08000000,
        0x08020200, 0x08000000, 0x00020008, 0x00000208,
        0x00020000, 0x08020200, 0x08000200, 0x00000000,
        0x00000200, 0x00020008, 0x08020208, 0x08000200,
        0x08000008, 0x00000200, 0x00000000, 0x08020008,
        0x08000208, 0x00020000, 0x08000000, 0x08020208,
        0x00000008, 0x00020208, 0x00020200, 0x08000008,
        0x08020000, 0x08000208, 0x00000208, 0x08020000,
        0x00020208, 0x00000008, 0x08020008, 0x00020200
    };

    static uint[] SB4 =
    {
        0x00802001, 0x00002081, 0x00002081, 0x00000080,
        0x00802080, 0x00800081, 0x00800001, 0x00002001,
        0x00000000, 0x00802000, 0x00802000, 0x00802081,
        0x00000081, 0x00000000, 0x00800080, 0x00800001,
        0x00000001, 0x00002000, 0x00800000, 0x00802001,
        0x00000080, 0x00800000, 0x00002001, 0x00002080,
        0x00800081, 0x00000001, 0x00002080, 0x00800080,
        0x00002000, 0x00802080, 0x00802081, 0x00000081,
        0x00800080, 0x00800001, 0x00802000, 0x00802081,
        0x00000081, 0x00000000, 0x00000000, 0x00802000,
        0x00002080, 0x00800080, 0x00800081, 0x00000001,
        0x00802001, 0x00002081, 0x00002081, 0x00000080,
        0x00802081, 0x00000081, 0x00000001, 0x00002000,
        0x00800001, 0x00002001, 0x00802080, 0x00800081,
        0x00002001, 0x00002080, 0x00800000, 0x00802001,
        0x00000080, 0x00800000, 0x00002000, 0x00802080
    };

    static uint[] SB5 =
    {
        0x00000100, 0x02080100, 0x02080000, 0x42000100,
        0x00080000, 0x00000100, 0x40000000, 0x02080000,
        0x40080100, 0x00080000, 0x02000100, 0x40080100,
        0x42000100, 0x42080000, 0x00080100, 0x40000000,
        0x02000000, 0x40080000, 0x40080000, 0x00000000,
        0x40000100, 0x42080100, 0x42080100, 0x02000100,
        0x42080000, 0x40000100, 0x00000000, 0x42000000,
        0x02080100, 0x02000000, 0x42000000, 0x00080100,
        0x00080000, 0x42000100, 0x00000100, 0x02000000,
        0x40000000, 0x02080000, 0x42000100, 0x40080100,
        0x02000100, 0x40000000, 0x42080000, 0x02080100,
        0x40080100, 0x00000100, 0x02000000, 0x42080000,
        0x42080100, 0x00080100, 0x42000000, 0x42080100,
        0x02080000, 0x00000000, 0x40080000, 0x42000000,
        0x00080100, 0x02000100, 0x40000100, 0x00080000,
        0x00000000, 0x40080000, 0x02080100, 0x40000100
    };

    static uint[] SB6 =
    {
        0x20000010, 0x20400000, 0x00004000, 0x20404010,
        0x20400000, 0x00000010, 0x20404010, 0x00400000,
        0x20004000, 0x00404010, 0x00400000, 0x20000010,
        0x00400010, 0x20004000, 0x20000000, 0x00004010,
        0x00000000, 0x00400010, 0x20004010, 0x00004000,
        0x00404000, 0x20004010, 0x00000010, 0x20400010,
        0x20400010, 0x00000000, 0x00404010, 0x20404000,
        0x00004010, 0x00404000, 0x20404000, 0x20000000,
        0x20004000, 0x00000010, 0x20400010, 0x00404000,
        0x20404010, 0x00400000, 0x00004010, 0x20000010,
        0x00400000, 0x20004000, 0x20000000, 0x00004010,
        0x20000010, 0x20404010, 0x00404000, 0x20400000,
        0x00404010, 0x20404000, 0x00000000, 0x20400010,
        0x00000010, 0x00004000, 0x20400000, 0x00404010,
        0x00004000, 0x00400010, 0x20004010, 0x00000000,
        0x20404000, 0x20000000, 0x00400010, 0x20004010
    };

    static uint[] SB7 =
    {
        0x00200000, 0x04200002, 0x04000802, 0x00000000,
        0x00000800, 0x04000802, 0x00200802, 0x04200800,
        0x04200802, 0x00200000, 0x00000000, 0x04000002,
        0x00000002, 0x04000000, 0x04200002, 0x00000802,
        0x04000800, 0x00200802, 0x00200002, 0x04000800,
        0x04000002, 0x04200000, 0x04200800, 0x00200002,
        0x04200000, 0x00000800, 0x00000802, 0x04200802,
        0x00200800, 0x00000002, 0x04000000, 0x00200800,
        0x04000000, 0x00200800, 0x00200000, 0x04000802,
        0x04000802, 0x04200002, 0x04200002, 0x00000002,
        0x00200002, 0x04000000, 0x04000800, 0x00200000,
        0x04200800, 0x00000802, 0x00200802, 0x04200800,
        0x00000802, 0x04000002, 0x04200802, 0x04200000,
        0x00200800, 0x00000000, 0x00000002, 0x04200802,
        0x00000000, 0x00200802, 0x04200000, 0x00000800,
        0x04000002, 0x04000800, 0x00000800, 0x00200002
    };

    static uint[] SB8 =
    {
        0x10001040, 0x00001000, 0x00040000, 0x10041040,
        0x10000000, 0x10001040, 0x00000040, 0x10000000,
        0x00040040, 0x10040000, 0x10041040, 0x00041000,
        0x10041000, 0x00041040, 0x00001000, 0x00000040,
        0x10040000, 0x10000040, 0x10001000, 0x00001040,
        0x00041000, 0x00040040, 0x10040040, 0x10041000,
        0x00001040, 0x00000000, 0x00000000, 0x10040040,
        0x10000040, 0x10001000, 0x00041040, 0x00040000,
        0x00041040, 0x00040000, 0x10041000, 0x00001000,
        0x00000040, 0x10040040, 0x00001000, 0x00041040,
        0x10001000, 0x00000040, 0x10000040, 0x10040000,
        0x10040040, 0x10000000, 0x00040000, 0x10001040,
        0x00000000, 0x10041040, 0x00040040, 0x10000040,
        0x10040000, 0x10001000, 0x10001040, 0x00000000,
        0x10041040, 0x00041000, 0x00041000, 0x00001040,
        0x00001040, 0x00040040, 0x10000000, 0x10041000
    };
    /*
     * PC1: left and right halves bit-swap
     */
    static uint[] LHs =
    {
        0x00000000, 0x00000001, 0x00000100, 0x00000101,
        0x00010000, 0x00010001, 0x00010100, 0x00010101,
        0x01000000, 0x01000001, 0x01000100, 0x01000101,
        0x01010000, 0x01010001, 0x01010100, 0x01010101
    };

    static uint[] RHs =
    {
        0x00000000, 0x01000000, 0x00010000, 0x01010000,
        0x00000100, 0x01000100, 0x00010100, 0x01010100,
        0x00000001, 0x01000001, 0x00010001, 0x01010001,
        0x00000101, 0x01000101, 0x00010101, 0x01010101,
    };    

    public TripleDes(byte[] key,bool encrypt) {
        context=new Des3Context();
        if(encrypt)des3Set2keyEnc(key);
        else des3Set2keyDec(key);
    }
    
    public static int CRC16_1021( int usCRC, byte[] sBuffer, int usLen )
    {
        int  usPtr, usTmp;

        for ( usPtr = 0 ; usPtr < usLen ; usPtr++ )
        {
            usTmp  = (ushort)(( ( usCRC >> 8 ) ^ sBuffer[ usPtr ] ) & 0xFF);
            usTmp ^= (ushort)(usTmp >> 4);
            usCRC = (ushort)((usCRC << 8) ^ (usTmp << 12) ^ (usTmp << 5) ^ usTmp); 
        }
	        return usCRC;
    }

    byte[] fillPadding( byte[] buffer){
        if((buffer.Length%8)>0){
            //byte[] fill= {(byte)0xCA,(byte)0xCA,(byte)0xCA,(byte)0xCA,(byte)0xCA,(byte)0xCA,(byte)0xCA};
            byte[] output= new byte[buffer.Length+(8-buffer.Length%8)];
            buffer.CopyTo(output, 0);
            //System.arraycopy(buffer, 0, output, 0, buffer.length);
            for (int i = buffer.Length; i < output.Length; i++)
            {
                output[i] = 0xCA;
            }
            //System.arraycopy(fill, 0, output, buffer.length, (8 - buffer.length % 8));
            return output;
        }
        return buffer;
    }    
    public byte[] des3_crypt_cbc( /*&ctx3,*/bool encrypt, byte[] iv,byte[] input){
        if(iv==null) iv=new byte[]{0,0,0,0,0,0,0,0};
        try {
            if( encrypt == DES_ENCRYPT )
            {
                input=fillPadding(input);
                byte[] data = new byte[input.Length];
                //ByteArrayOutputStream encrypted = new ByteArrayOutputStream();
                for(int largo=0; largo<input.Length;largo+=8){
                    byte[] output= new byte[(input.Length-largo)>8 ? 8:(input.Length-largo)];
                    for(int i = 0; i < output.Length; i++ ) output[i] = (byte)(input[i+largo] ^ iv[i]);
                    output=des3_crypt_ecb(output);
                    //encrypted.write(output);
                    output.CopyTo(data, largo);
                    iv=output;
                }
                //byte[] data=encrypted.toByteArray();
                //encrypted.close();
                return data;
            }
            else{
                byte[] data = new byte[input.Length];
                //ByteArrayOutputStream encrypted = new ByteArrayOutputStream();
                for(int largo=0; largo<input.Length;largo+=8){
                    byte[] temp = new byte[8];
                    byte[] output= new byte[(input.Length-largo)>8 ? 8:(input.Length-largo)];
                    for(int i = 0; i < output.Length; i++ ){
                        temp[i]=input[i+largo];
                        output[i] = input[i+largo];
                    }
                    output=des3_crypt_ecb(output);
                    for(int i = 0; i < output.Length; i++ ){
                        output[i] = (byte)(output[i] ^ iv[i]);
                    }
                    output.CopyTo(data, largo);
                    //encrypted.write(output);
                    iv=temp;
                }
                //byte[] data=encrypted.toByteArray();
                //encrypted.close();
                return data;
            }
        }
        catch (Exception ex) {
            
        }
        return null;
    }

    public byte[] removePadding(byte[] input)
    {
        int outCounter = 0;
        byte[] output = new byte[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] != 0xCA)
            {
                output[outCounter] = input[i];
                outCounter++;
            }
        }
        Array.Resize<byte>(ref output, outCounter);
        return output;
    }

    private byte[] des3_crypt_ecb(byte[] input) {
        Aux x= new Aux();
        Aux y= new Aux();
        Aux t= new Aux();
        Aux contextIndex=new Aux();
        x.value=getInt(input, 0);
        y.value=getInt(input, 4);
        DES_IP(ref x, ref  y, ref  t);
        for( int i = 0; i < 8; i++ )
        {
            DES_ROUND(ref  y, ref  x, ref  t, ref  context.sk, ref  contextIndex);
            DES_ROUND(ref  x, ref  y, ref  t, ref  context.sk, ref  contextIndex);
        }
        for(int i = 0; i < 8; i++ )
        {
            DES_ROUND(ref  x, ref  y, ref  t, ref  context.sk, ref  contextIndex);
            DES_ROUND(ref  y, ref  x, ref  t, ref  context.sk, ref  contextIndex);
        }

        for(int i = 0; i < 8; i++ )
        {
            DES_ROUND(ref  y, ref  x, ref  t, ref  context.sk, ref  contextIndex);
            DES_ROUND(ref  x, ref  y, ref  t, ref  context.sk, ref  contextIndex);
        }

        DES_FP(ref  y, ref  x, ref  t);

        byte[] output = new byte[8];
        PUT_ULONG_BE(ref  y.value, ref  output, 0);
        PUT_ULONG_BE(ref  x.value, ref  output, 4);
        
        return output;
    }
//    long getInt(byte[] input, int offset){
//        return (long)(((long)(input[offset]&0xFF)<<24)|(long)((input[offset+1]&0xFF)<<16)|(long)((input[offset+2]&0xFF)<<8)|(long)((input[offset+3]&0xFF)));
//    }
    uint getInt(byte[] input, uint offset)
    {
        return (uint)(((input[offset]&0xFF)<<24)|((input[offset+1]&0xFF)<<16)|((input[offset+2]&0xFF)<<8)|((input[offset+3]&0xFF)));
    }
    void DES_IP(ref Aux x,ref Aux y,ref Aux t ){
        t.value = ((x.value>>4)^y.value) & 0x0F0F0F0F;
        y.value ^= t.value; 
        x.value ^= (t.value<< 4);
        t.value = ((x.value >> 16) ^ y.value) & 0x0000FFFF; 
        y.value ^= t.value; 
        x.value ^= (t.value << 16);
        t.value = ((y.value >>  2) ^ x.value) & 0x33333333; 
        x.value ^= t.value; 
        y.value ^= (t.value <<  2);
        t.value = ((y.value >>  8) ^ x.value) & 0x00FF00FF; 
        x.value ^= t.value; 
        y.value ^= (t.value <<  8);
        y.value = ((y.value << 1) | (y.value >> 31)) & 0xFFFFFFFF;
        t.value = (x.value ^ y.value) & 0xAAAAAAAA; 
        y.value ^= t.value; 
        x.value ^= t.value;
        x.value = ((x.value << 1) | (x.value >> 31)) & 0xFFFFFFFF;
    }
    void DES_ROUND(ref Aux x, ref Aux y, ref Aux t, ref uint[] sk, ref Aux index)
    {
        t.value = (sk[index.value++] ^ x.value);
        y.value^= (SB8[(t.value&0x3F)]^SB6[((t.value>>8)&0x3F)]^SB4[((t.value>>16)&0x3F)]^SB2[((t.value>>24)&0x3F)]);
        t.value= (sk[index.value++]^((x.value<<28)|(x.value>>4)));
        y.value^= (SB7[(t.value&0x3F)]^SB5[((t.value>>8)&0x3F)]^SB3[((t.value>>16)&0x3F)]^SB1[((t.value>>24)&0x3F)]);
    }    
    /*
     * Final Permutation macro
    */
    void DES_FP(ref Aux x,ref Aux y,ref Aux t){
        x.value = ((x.value << 31) | (x.value >> 1)) & 0xFFFFFFFF;
        t.value = (x.value ^ y.value) & 0xAAAAAAAA; x.value ^= t.value; 
        y.value ^= t.value;
        y.value = ((y.value << 31) | (y.value >> 1)) & 0xFFFFFFFF;
        t.value = ((y.value >>  8) ^ x.value) & 0x00FF00FF; 
        x.value ^= t.value; 
        y.value ^= (t.value <<  8);
        t.value = ((y.value >>  2) ^ x.value) & 0x33333333; 
        x.value ^= t.value; 
        y.value ^= (t.value <<  2);
        t.value = ((x.value >> 16) ^ y.value) & 0x0000FFFF; 
        y.value ^= t.value; 
        x.value ^= (t.value << 16);
        t.value = ((x.value >>  4) ^ y.value) & 0x0F0F0F0F; 
        y.value ^= t.value; 
        x.value ^= (t.value <<  4);
    }
    void PUT_ULONG_BE(ref uint n, ref byte[] b, uint offset)
    {
        b[offset] = (byte) ((n >> 24)&0xFF );
        b[offset+1] = (byte) ((n >> 16)&0xFF );
        b[offset+2] = (byte) ((n >>  8)&0xFF );
        b[offset+3] = (byte) (n&0xFF);
    }
    void desSetkey(ref uint[] sk, uint offsetSK, ref byte[] key, uint offsetKey)
    {
        int i;
        uint indice = offsetSK;
        uint x, y, t;

        x=getInt(key, 0+offsetKey );
        y=getInt(key, 4+offsetKey );

        /*
         * Permuted Choice 1
         */
        t =  ((y>>4)^x)&0x0F0F0F0F; 
        x ^= t; 
        y ^= (t<<4);
        t =  (y^x)&0x10101010;  
        x ^= t; 
        y ^= t;
        x = ((LHs[(x&0xF)]<<3)|(LHs[((x>>8)&0xF)]<<2)
            | (LHs[((x>>16)&0xF)]<<1)|(LHs[((x>>24)&0xF)])
            | (LHs[((x>>5)&0xF)]<<7)|(LHs[((x>>13)&0xF)]<<6)
            | (LHs[((x>>21)&0xF)]<<5)|(LHs[((x>>29)&0xF)]<<4));

        y =   (RHs[((y>>1)&0xF)]<<3)|(RHs[((y>>9)&0xF)]<<2)
            |(RHs[((y>>17)&0xF)]<<1)|(RHs[((y>>25)&0xF)])
            |(RHs[((y>>4)&0xF)]<<7)|(RHs[((y>>12)&0xF)]<<6)
            |(RHs[((y>>20)&0xF)]<<5)|(RHs[((y>>28)&0xF)]<<4);

        x &= 0x0FFFFFFF;
        y &= 0x0FFFFFFF;

        /*
         * calculate subkeys
         */
        for( i = 0; i < 16; i++ )
        {
            if( i < 2 || i == 8 || i == 15 )
            {
                x = ((x <<  1) | (x >> 27)) & 0x0FFFFFFF;
                y = ((y <<  1) | (y >> 27)) & 0x0FFFFFFF;
            }
            else
            {
                x = ((x<<2)|(x>>26))&0x0FFFFFFF;
                y = ((y<<2)|(y>>26))&0x0FFFFFFF;
            }

            sk[indice++] =   ((x <<  4) & 0x24000000) | ((x << 28) & 0x10000000)
                    | ((x << 14) & 0x08000000) | ((x << 18) & 0x02080000)
                    | ((x <<  6) & 0x01000000) | ((x <<  9) & 0x00200000)
                    | ((x >>  1) & 0x00100000) | ((x << 10) & 0x00040000)
                    | ((x <<  2) & 0x00020000) | ((x >> 10) & 0x00010000)
                    | ((y >> 13) & 0x00002000) | ((y >>  4) & 0x00001000)
                    | ((y <<  6) & 0x00000800) | ((y >>  1) & 0x00000400)
                    | ((y >> 14) & 0x00000200) | ((y      ) & 0x00000100)
                    | ((y >>  5) & 0x00000020) | ((y >> 10) & 0x00000010)
                    | ((y >>  3) & 0x00000008) | ((y >> 18) & 0x00000004)
                    | ((y >> 26) & 0x00000002) | ((y >> 24) & 0x00000001);

            sk[indice++] =   ((x << 15) & 0x20000000) | ((x << 17) & 0x10000000)
                    | ((x << 10) & 0x08000000) | ((x << 22) & 0x04000000)
                    | ((x >>  2) & 0x02000000) | ((x <<  1) & 0x01000000)
                    | ((x << 16) & 0x00200000) | ((x << 11) & 0x00100000)
                    | ((x <<  3) & 0x00080000) | ((x >>  6) & 0x00040000)
                    | ((x << 15) & 0x00020000) | ((x >>  4) & 0x00010000)
                    | ((y >>  2) & 0x00002000) | ((y <<  8) & 0x00001000)
                    | ((y >> 14) & 0x00000808) | ((y >>  9) & 0x00000400)
                    | ((y      ) & 0x00000200) | ((y <<  7) & 0x00000100)
                    | ((y >>  7) & 0x00000020) | ((y >>  3) & 0x00000011)
                    | ((y <<  2) & 0x00000004) | ((y >> 21) & 0x00000002);
        }
    }    
    private void des3Set2keyEnc( byte[] key ){
        int i;
        uint[] dsk = new uint[96];
        desSetkey(ref context.sk,0,ref key,0);
        desSetkey(ref dsk,32,ref key,8);

        for( i = 0; i < 32; i += 2 )
        {
            dsk[i     ] = context.sk[30 - i];
            dsk[i +  1] = context.sk[31 - i];

            context.sk[i + 32] = dsk[62 - i];
            context.sk[i + 33] = dsk[63 - i];

            context.sk[i + 64] = context.sk[i];
            context.sk[i + 65] = context.sk[i+1];

            dsk[i + 64] = dsk[i    ];
            dsk[i + 65] = dsk[i + 1];
        }
    }
    private void des3Set2keyDec( byte[] key ){
        int i;
        uint[] dsk = new uint[96];
        desSetkey(ref dsk, 0, ref key, 0);
        desSetkey(ref context.sk, 32, ref key, 8);

        for( i = 0; i < 32; i += 2 )
        {
            context.sk[i     ] = dsk[30 - i];
            context.sk[i +  1] = dsk[31 - i];

            dsk[i + 32] = context.sk[62 - i];
            dsk[i + 33] = context.sk[63 - i];

            dsk[i + 64] = dsk[i];
            dsk[i + 65] = dsk[i+1];

            context.sk[i + 64] = context.sk[i    ];
            context.sk[i + 65] = context.sk[i + 1];
        }
    }    
    }
}
