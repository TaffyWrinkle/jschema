﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using FluentAssertions;
using Xunit;

namespace Microsoft.Json.Schema.UnitTests
{
    public class JsonSchemaTests
    {
        public static readonly TheoryData<EqualityTestCase> EqualityTestCases = new TheoryData<EqualityTestCase>
        {
            new EqualityTestCase(
                "Empty schemas",
                @"{}",
                @"{}",
                true
            ),

            new EqualityTestCase(
                "All properties equal",
                @"{
                    ""id"": ""http://x/y#"",
                    ""$schema"": ""http://z"",
                    ""title"": ""x"",
                    ""enum"": [ ""a"", ""b"" ],
                    ""items"": {
                      ""type"": ""integer""
                    },
                    ""properties"": {
                      ""a"": {
                        ""type"": ""object""
                      },
                      ""b"": {
                        ""type"": ""string""
                      }
                    },
                    ""required"": [ ""a"" ],
                    ""definitions"": {
                      ""c"": {
                        ""type"": ""integer""
                      },
                      ""d"": {
                        ""type"": ""boolean""
                      }
                    },
                    ""additionalProperties"": true,
                    ""$ref"": ""http://www.example.com/schema/#"",
                    ""minItems"": 1,
                    ""maxItems"": 3,
                    ""format"": ""date-time""
                }",
                @"{
                    ""id"": ""http://x/y#"",
                    ""$schema"": ""http://z"",
                    ""title"": ""x"",
                    ""enum"": [ ""a"", ""b"" ],
                    ""items"": {
                      ""type"": ""integer""
                    },
                    ""properties"": {
                      ""a"": {
                        ""type"": ""object""
                      },
                      ""b"": {
                        ""type"": ""string""
                      }
                    },
                    ""required"": [ ""a"" ],
                    ""definitions"": {
                      ""c"": {
                        ""type"": ""integer""
                      },
                      ""d"": {
                        ""type"": ""boolean""
                      }
                    },
                    ""additionalProperties"": true,
                    ""$ref"": ""http://www.example.com/schema/#"",
                    ""minItems"": 1,
                    ""maxItems"": 3,
                    ""format"": ""date-time""
                }",
                true
            ),

            new EqualityTestCase(
                "Different Ids",
                @"{
                  ""id"": ""http://x/y#"",
                }",
                @"{
                  ""id"": ""http://x/y#a"",
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null Ids",
                @"{}",
                @"{
                  ""id"": ""http://x/y#"",
                }",
                false
            ),

            new EqualityTestCase(
                "Different schema versions",
                @"{
                  ""$schema"": ""http://z""
                }",
                @"{
                  ""$schema"": ""http://q""
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null schema versions",
                @"{}",
                @"{
                  ""$schema"": ""http://z""
                }",
                false
            ),

            new EqualityTestCase(
                "Different titles",
                @"{
                  ""title"": ""x""
                }",
                @"{
                  ""title"": ""y""
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null titles",
                @"{}",
                @"{
                  ""title"": ""y""
                }",
                false
            ),

            new EqualityTestCase(
                "Different enum lists",
                @"{
                  ""enum"": [ ""a"", ""b"" ]
                }",
                @"{
                  ""enum"": [ ""a"", ""c"" ]
                }",
                false
            ),

            new EqualityTestCase(
                "Same enum lists in different orders",
                @"{
                  ""enum"": [ ""a"", ""b"" ]
                }",
                @"{
                  ""enum"": [ ""b"", ""a"" ]
                }",
                true
            ),

            new EqualityTestCase(
                "Null and non-null enum lists",
                @"{}",
                @"{
                  ""enum"": [ ""a"", ""b"" ]
                }",
                false
            ),

            new EqualityTestCase(
                "Different item schemas",
                @"{
                  ""items"": {
                    ""type"": ""integer""
                  }
                }",
                @"{
                  ""items"": {
                    ""type"": ""boolean""
                  }
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null item schemas",
                @"{}",
                @"{
                  ""items"": {
                    ""type"": ""boolean""
                  }
                }",
                false
            ),

            new EqualityTestCase(
                "Different property schemas",
                @"{
                  ""properties"": {
                    ""a"": {
                      ""type"": ""object""
                    },
                    ""b"": {
                      ""type"": ""string""
                    }
                  }
                }",
                @"{
                  ""properties"": {
                    ""a"": {
                      ""type"": ""object""
                    },
                    ""b"": {
                      ""type"": ""float""
                    }
                  }
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null property sets",
                @"{}",
                @"{
                  ""properties"": {
                    ""a"": {
                      ""type"": ""object""
                    },
                    ""b"": {
                      ""type"": ""float""
                    }
                  }
                }",
                false
            ),

            new EqualityTestCase(
                "Different required properties",
                @"{
                  ""required"": [ ""a"", ""b"" ]
                }",
                @"{
                  ""required"": [ ""a"" ]
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null required properties",
                @"{}",
                @"{
                  ""required"": [ ""a"" ]
                }",
                false
            ),

            new EqualityTestCase(
                "Different definitions dictionaries",
                @"{
                  ""definitions"": {
                    ""c"": {
                      ""type"": ""integer""
                    },
                    ""d"": {
                      ""type"": ""boolean""
                    }
                  }
                }",
                @"{
                  ""definitions"": {
                    ""e"": {
                      ""type"": ""integer""
                    },
                    ""f"": {
                      ""type"": ""boolean""
                    }
                  }
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null definitions dictionaries",
                @"{}",
                @"{
                  ""definitions"": {
                    ""e"": {
                      ""type"": ""integer""
                    },
                    ""f"": {
                      ""type"": ""boolean""
                    }
                  }
                }",
                false
            ),

            new EqualityTestCase(
                "Different references",
                @"{
                  ""$ref"": ""schema/#""
                }",
                @"{
                  ""$ref"": ""schema/#x""
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null references",
                @"{}",
                @"{
                  ""$ref"": ""schema/#x""
                }",
                false
            ),

            new EqualityTestCase(
                "Different minimum array lengths",
                @"{
                  ""minItems"": 1
                }",
                @"{
                  ""minItems"": 2
                }",
                false
            ),

            // These two schemas would validate the same set of instances, but
            // we consider them unequal because they serialize to different
            // JSON schema strings.
            new EqualityTestCase(
                "Missing and zero minimum array lengths",
                @"{}",
                @"{
                  ""minItems"": 0
                }",
                false
            ),

            new EqualityTestCase(
                "Different maximum array lengths",
                @"{
                  ""maxItems"": 1
                }",
                @"{
                  ""maxItems"": 2
                }",
                false
            ),

            // These two schemas would validate the same set of instances, but
            // we consider them unequal because they serialize to different
            // JSON schema strings.
            new EqualityTestCase(
                "Missing and zero maximum array lengths",
                @"{}",
                @"{
                  ""maxItems"": 0
                }",
                false
            ),

            new EqualityTestCase(
                "Different formats",
                @"{
                  ""format"": ""data-time""
                }",
                @"{
                  ""format"": ""email""
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null formats",
                @"{}",
                @"{
                  ""format"": ""data-time""
                }",
                false
            ),

            new EqualityTestCase(
                "Null and non-null additional properties",
                @"{}",
                @"{
                  ""additionalProperties"": true
                }",
                false
            ),

            new EqualityTestCase(
                "Additional properties with Boolean value and schema",
                @"{
                  ""additionalProperties"": true
                }",
                @"{
                  ""additionalProperties"": {}
                }",
                false
            ),

            new EqualityTestCase(
                "Additional properties with different Boolean values",
                @"{
                  ""additionalProperties"": true
                }",
                @"{
                  ""additionalProperties"": false
                }",
                false
            ),

            new EqualityTestCase(
                "Additional properties with different schemas",
                @"{
                  ""additionalProperties"": {
                    ""format"": ""date-time""
                  }
                }",
                @"{
                  ""additionalProperties"": {}
                }",
                false
            ),
        };

        [Theory(DisplayName = "JsonSchema equality")]
        [MemberData(nameof(EqualityTestCases))]
        public void EqualityTests(EqualityTestCase testCase)
        {
            JsonSchema left = SchemaReader.ReadSchema(testCase.Left);
            JsonSchema right = SchemaReader.ReadSchema(testCase.Right);

            left.Equals(right).Should().Be(testCase.ShouldBeEqual);
            (left == right).Should().Be(testCase.ShouldBeEqual);
            (left != right).Should().Be(!testCase.ShouldBeEqual);
        }
    }
}
