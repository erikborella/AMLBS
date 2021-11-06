using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS
{
    enum TokenTypeEnum
    {
        NUMBER,
        WORD,
        SYMBOL,
        LEFT_PARENTHESIS,
        RIGHT_PARENTHESIS,
        SEPARATOR,
        ATTRIBUTION,
        END_EXPRESSION
    }


    record Token(TokenTypeEnum Type, object Value);
}
