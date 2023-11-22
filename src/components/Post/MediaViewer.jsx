import { Box } from '@mui/material';
import React from 'react'
import DocViewer, { DocViewerRenderers } from '@cyntler/react-doc-viewer';

function MediaViewer({ source, closeDoc }) {
    const docs = [
        { uri: source }, 
    ];

    function close(e) {
        e.stopPropagation();
        if (e.target.id === 'outside-doc') {
            closeDoc()
        }
    }

    return (
        <Box
            id='outside-doc'
            sx={{
                position: 'absolute',
                zIndex: 20,
                top: 0,
                left: 0,
                width: '100vw',
                height: '100vh',
                display: 'flex',
                justifyContent: 'center',
                backgroundColor: 'rgba(0,0,0,0.35)',
            }}
            onClick={close}
        >
            <DocViewer
                documents={docs}
                pluginRenderers={DocViewerRenderers}
                style={{ width: '60vw', height: '100vh', overflow: 'auto', }}
                config={{
                    header: {
                        disableHeader: false,
                        disableFileName: false,
                        retainURLParams: false,
                    },
                    csvDelimiter: ",", // "," as default,
                    pdfZoom: {
                        defaultZoom: 1.1, // 1 as default,
                        zoomJump: 0.2, // 0.1 as default,
                    },
                    pdfVerticalScrollByDefault: true, // false as default
                }}
                theme={{
                    primary: "#90A4AE",
                    secondary: "#ffffff",
                    tertiary: "#CFD8DC",
                    textPrimary: "#000000",
                    textSecondary: "#5296d8",
                    textTertiary: "#00000099",
                    disableThemeScrollbar: false,
                }}
            />
        </Box>
    );
}

export default MediaViewer