// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <spec>http://webdata/xml/specs/querylowlevel.xml</spec>
//------------------------------------------------------------------------------

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl.Runtime;
using System.Runtime.Versioning;

namespace System.Xml.Xsl
{
    /// <summary>
    /// This is the executable command generated by the XmlILGenerator.
    /// </summary>
    internal class XmlILCommand
    {
        private ExecuteDelegate _delExec;
        private XmlQueryStaticData _staticData;

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlILCommand(ExecuteDelegate delExec, XmlQueryStaticData staticData)
        {
            Debug.Assert(delExec != null && staticData != null);
            _delExec = delExec;
            _staticData = staticData;
        }

        /// <summary>
        /// Return execute delegate.
        /// </summary>
        public ExecuteDelegate ExecuteDelegate
        {
            get { return _delExec; }
        }

        /// <summary>
        /// Return query static data required by the runtime.
        /// </summary>
        public XmlQueryStaticData StaticData
        {
            get { return _staticData; }
        }

#if false
        /// <summary>
        /// Default serialization options that will be used if the user does not supply an XmlWriter
        /// at execution time.
        /// </summary>
        public override XmlWriterSettings DefaultWriterSettings {
            get { return this.staticData.DefaultWriterSettings; }
        }

        /// <summary>
        /// Default document as XPathNavigator.
        /// </summary>
        public override void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results) {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            if (contextDocument != null)
                Execute(contextDocument.CreateNavigator(), dataSources, argumentList, results, false);
            else
                Execute(null, dataSources, argumentList, results, false);
        }

        /// <summary>
        /// Default document as XPathNavigator.
        /// </summary>
        public override void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, TextWriter results) {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings));
        }

        /// <summary>
        /// Default document as XPathNavigator.
        /// </summary>
        public override void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, Stream results) {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings));
        }

        /// <summary>
        /// Executes the query by accessing datasources via the XmlResolver and using run-time parameters
        /// as provided by the XsltArgumentList. The default document is mapped into the XmlResolver with the
        /// provided name. The results are output to the provided XmlWriter.
        /// </summary>
        public void Execute(string contextDocumentUri, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results) {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Execute(contextDocumentUri, dataSources, argumentList, results, false);
        }
#endif

        /// <summary>
        /// Executes the query by accessing datasources via the XmlResolver and using
        /// run-time parameters as provided by the XsltArgumentList. The default document
        /// is mapped into the XmlResolver with the provided name. The results are returned
        /// as an IList.
        /// </summary>
        public IList Evaluate(string contextDocumentUri, XmlResolver dataSources, XsltArgumentList argumentList)
        {
            XmlCachedSequenceWriter seqwrt = new XmlCachedSequenceWriter();
            Execute(contextDocumentUri, dataSources, argumentList, seqwrt);
            return seqwrt.ResultSequence;
        }

#if false
        /// <summary>
        /// Executes the query by accessing datasources via the XmlResolver and using run-time parameters
        /// as provided by the XsltArgumentList. The default document is mapped into the XmlResolver with the
        /// provided name. The results are output to the provided XmlWriter.
        /// </summary>
        public override void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results) {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Execute(contextDocument, dataSources, argumentList, results, false);
        }

        /// <summary>
        /// Executes the query by accessing datasources via the XmlResolver and using run-time parameters
        /// as provided by the XsltArgumentList. The default document is mapped into the XmlResolver with the
        /// provided name. The results are output to the provided TextWriter.
        /// </summary>
        public override void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, TextWriter results) {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings), true);
        }

        /// <summary>
        /// Executes the query by accessing datasources via the XmlResolver and using run-time parameters
        /// as provided by the XsltArgumentList. The default document is mapped into the XmlResolver with the
        /// provided name. The results are output to the provided Stream.
        /// </summary>
        public override void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, Stream results) {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings), true);
        }

        /// <summary>
        /// Executes the query by accessing datasources via the XmlResolver and using
        /// run-time parameters as provided by the XsltArgumentList. The default document
        /// is mapped into the XmlResolver with the provided name. The results are returned
        /// as an IList.
        /// </summary>
        public override IList Evaluate(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList) {
            XmlCachedSequenceWriter seqwrt = new XmlCachedSequenceWriter();
            Execute(contextDocument, dataSources, argumentList, seqwrt);
            return seqwrt.ResultSequence;
        }
#endif

        /// <summary>
        /// Execute the dynamic assembly generated by the XmlILGenerator.
        /// </summary>
        public void Execute(object defaultDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter writer)
        {
            try
            {
                if (writer is XmlAsyncCheckWriter)
                {
                    writer = ((XmlAsyncCheckWriter)writer).CoreWriter;
                }

                // Try to extract a RawWriter
                XmlWellFormedWriter wellFormedWriter = writer as XmlWellFormedWriter;

                if (wellFormedWriter != null &&
                    wellFormedWriter.RawWriter != null &&
                    wellFormedWriter.WriteState == WriteState.Start &&
                    wellFormedWriter.Settings.ConformanceLevel != ConformanceLevel.Document)
                {
                    // Extracted RawWriter from WellFormedWriter
                    Execute(defaultDocument, dataSources, argumentList, new XmlMergeSequenceWriter(wellFormedWriter.RawWriter));
                }
                else
                {
                    // Wrap Writer in RawWriter
                    Execute(defaultDocument, dataSources, argumentList, new XmlMergeSequenceWriter(new XmlRawWriterWrapper(writer)));
                }
            }
            finally
            {
                writer.Flush();
            }
        }

        /// <summary>
        /// Execute the dynamic assembly generated by the XmlILGenerator.
        /// </summary>
        private void Execute(object defaultDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlSequenceWriter results)
        {
            Debug.Assert(results != null);

            // Ensure that dataSources is always non-null
            if (dataSources == null)
                dataSources = XmlNullResolver.Singleton;

            _delExec(new XmlQueryRuntime(_staticData, defaultDocument, dataSources, argumentList, results));
        }
    }
}
