﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Json.Schema.UnitTests
{
    public class ValidatorTests
    {
        public class TestCase : IXunitSerializable
        {
            public TestCase(
                string name,
                string schemaText,
                string instanceText,
                params string[] expectedMessages)
            {
                Name = name;
                SchemaText = schemaText;
                InstanceText = instanceText;
                ExpectedMessages = expectedMessages;
            }

            public TestCase()
            {
                // Needer for deserializer.
            }

            public string Name;
            public string SchemaText;
            public string InstanceText;
            public string[] ExpectedMessages;

            public override string ToString()
            {
                return Name;
            }

            public void Deserialize(IXunitSerializationInfo info)
            {
                Name = info.GetValue<string>(nameof(Name));
                SchemaText = info.GetValue<string>(nameof(SchemaText));
                InstanceText = info.GetValue<string>(nameof(InstanceText));
                ExpectedMessages = info.GetValue<string[]>(nameof(ExpectedMessages));
            }

            public void Serialize(IXunitSerializationInfo info)
            {
                info.AddValue(nameof(Name), Name);
                info.AddValue(nameof(SchemaText), SchemaText);
                info.AddValue(nameof(InstanceText), InstanceText);
                info.AddValue(nameof(ExpectedMessages), ExpectedMessages);
            }
        }

        public static TheoryData<TestCase> TestCases = new TheoryData<TestCase>
        {
            new TestCase(
                "Empty schema matches integer",
                "{}",
                "2"
                ),

            new TestCase(
                "Empty schema matches object",
                "{}",
                "{}"
                ),

            new TestCase(
                "Integer instance matches integer schema",
                @"{ ""type"": ""integer"" }",
                "2"
                ),

            new TestCase(
                "Non-integer instance does not match integer schema",
                @"{ ""type"": ""integer"" }",
                "\"s\"",
                ErrorMessage.Format(1, 3, ErrorNumber.WrongType, Validator.RootObjectName, JTokenType.Integer, JTokenType.String)
                ),

            new TestCase(
                "Array instance matches array schema",
                @"{ ""type"": ""array"" }",
                "[]"
                ),

            new TestCase(
                "Non-array instance matches array schema",
                 @"{ ""type"": ""array"" }",
                "true",
                ErrorMessage.Format(1, 4, ErrorNumber.WrongType, Validator.RootObjectName, JTokenType.Array, JTokenType.Boolean)
                ),

            new TestCase(
                "Integer instance matches number schema",
                @"{ ""type"": ""number"" }",
                "2"
                ),

            new TestCase(
                "Array has valid minimum number of items",
                @"{
                  ""type"": ""array"",
                  ""minItems"": 2,
                  ""maxItems"": 4
                }",
                "[ 1, 2 ]"
                ),

            new TestCase(
                "Array has valid maximum number of items",
                @"{
                  ""type"": ""array"",
                  ""minItems"": 2,
                  ""maxItems"": 4
                }",
                "[ 1, 2, 3, 4 ]"
                ),

            new TestCase(
                "Array has too few items",
                @"{
                  ""type"": ""array"",
                  ""minItems"": 2,
                  ""maxItems"": 4
                }",
                "[ 1 ]",
                ErrorMessage.Format(1, 1, ErrorNumber.TooFewArrayItems, 2, 1)
                ),

            new TestCase(
                "Array has too many items",
                @"{
                  ""type"": ""array"",
                  ""minItems"": 2,
                  ""maxItems"": 4
                }",
                "[ 1, 2, 3, 4, 5 ]",
                ErrorMessage.Format(1, 1, ErrorNumber.TooManyArrayItems, 4, 5)
                ),

            new TestCase(
                "Array without length constraint",
                @"{
                  ""type"": ""array"",
                }",
                "[ 1, 2, 3 ]"
                ),

            new TestCase(
                "Required property missing",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""boolean""
    },
    ""b"": {
      ""type"": ""boolean""
    }
  },
  ""required"": [ ""a"", ""b"", ""c"" ]
}",

@"{
    ""b"": true
}",
                ErrorMessage.Format(1, 1, ErrorNumber.RequiredPropertyMissing, "a"),
                ErrorMessage.Format(1, 1, ErrorNumber.RequiredPropertyMissing, "c")
                ),

            new TestCase(
                "Object property matches its schema",

@"{
    ""type"": ""object"",
    ""properties"": {
    ""a"": {
        ""type"": ""boolean""
    }
  }
}",

@"{
  ""a"": true
}"
                ),

            new TestCase(
                "Object property does not match its schema",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""boolean""
    }
  }
}",

@"{
  ""a"": ""true""
}",
                ErrorMessage.Format(2, 14, ErrorNumber.WrongType, "a", JTokenType.Boolean, JTokenType.String)
                ),

            new TestCase(
                "Object property missing required property",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""object"",
      ""properties"": {
        ""x"": {
          ""type"": ""integer""
        },
        ""y"": {
          ""type"": ""integer""
        },
        ""z"": {
          ""type"": ""integer""
        }
      },
      ""required"": [ ""x"", ""y"", ""z"" ]
    }
  }
}",

@"{
  ""a"": {
    ""y"": 2
  }
}",
                ErrorMessage.Format(2, 9, ErrorNumber.RequiredPropertyMissing, "x"),
                ErrorMessage.Format(2, 9, ErrorNumber.RequiredPropertyMissing, "z")
                ),

            new TestCase(
                "Object has additional property allowed by Boolean",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""integer""
    }
  },
  ""additionalProperties"": true
}",

@"{
  ""a"": 2,
  ""b"": {}
}"
                ),

            new TestCase(
                "Object has additional property disallowed by Boolean",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""integer""
    }
  },
  ""additionalProperties"": false
}",

@"{
  ""a"": 2,
  ""b"": {}
}",
                ErrorMessage.Format(3, 7, ErrorNumber.AdditionalPropertiesProhibited, "b")
                ),

            new TestCase(
                "Object has additional property disallowed by default",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""integer""
    }
  }
}",

@"{
  ""a"": 2,
  ""b"": {}
}",
                ErrorMessage.Format(3, 7, ErrorNumber.AdditionalPropertiesProhibited, "b")
                ),

            new TestCase(
                "Object has additional property allowed by schema",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""integer""
    }
  },
  ""additionalProperties"": {
    ""type"": ""boolean""
  }
}",

@"{
  ""a"": 2,
  ""b"": false
}"
                ),

            new TestCase(
                "Object has additional property disallowed by schema",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""integer""
    }
  },
  ""additionalProperties"": {
    ""type"": ""boolean""
  }
}",

@"{
  ""a"": 2,
  ""b"": ""false""
}",
                ErrorMessage.Format(3, 15, ErrorNumber.WrongType, "b", JTokenType.Boolean, JTokenType.String)
                ),

            new TestCase(
                "Nested object has additional property disallowed by schema",

@"{
  ""type"": ""object"",
  ""properties"": {
    ""a"": {
      ""type"": ""object"",
        ""additionalProperties"": {
          ""type"": ""boolean""
        }
      }
    }
  },
}",

@"{
  ""a"": {
    ""x"": 3
  }
}",
                ErrorMessage.Format(3, 11, ErrorNumber.WrongType, "a.x", JTokenType.Boolean, JTokenType.Integer)
                ),
        };

        [Theory(DisplayName = "Validation")]
        [MemberData(nameof(TestCases))]
        public void ReportsMissingRequiredProperty(TestCase test)
        {
            JsonSchema schema = SchemaReader.ReadSchema(test.SchemaText);
            var target = new Validator(schema);
            string[] actualMessages = target.Validate(test.InstanceText);

            actualMessages.Length.Should().Be(test.ExpectedMessages.Length);
            actualMessages.Should().ContainInOrder(test.ExpectedMessages);
        }
    }
}