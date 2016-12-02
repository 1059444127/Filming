//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dcm_print_object.h
///  \brief   dicom print object class. create stored print object and 
///           hardcopy grayscale object file.
///
///  \version 1.0
///  \date    Oct. 17, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef _MCSF_DCM_PRINT_OBJECT_H_
#define _MCSF_DCM_PRINT_OBJECT_H_

#pragma warning(disable:4819)
#pragma warning(disable:4512)
#pragma warning(disable:4267)

#include <vector>

#include "dcmtk/config/osconfig.h"   /* make sure OS specific configuration is included first */

#include "dcmtk/ofstd/ofstream.h"
#include "dcmtk/dcmpstat/dvpscf.h"     /* for class DVConfiguration */
#include "dcmtk/dcmpstat/dvpstat.h"    /* for class DVPresentationState */
#include "dcmtk/dcmqrdb/dcmqridx.h"    /* for struct IdxRecord */
#include "dcmtk/ofstd/ofstring.h"      /* for class OFString */
#include "dcmtk/dcmpstat/dvcache.h"    /* for index file caching */

#include "mcsf_dcm_film_session_object.h"
#include "mcsf_print_job_object.h"


class DicomImage;
class DiDisplayFunction;
class DVPSStoredPrint;
class DVPSPrintMessageHandler;
class DSRDocument;
class DVSignatureHandler;

MCSF_FILMING_BEGIN_NAMESPACE

/** Interface class for the Softcopy Presentation State viewer.
 *  This class manages the database facilities, allows to start and stop
 *  network transactions and gives access to images and presentation states.
 */
class McsfDcmPrintObject//: public DVConfiguration
{

public:
    McsfDcmPrintObject(McsfPrintJobObject* pPrintJobObject);

    /** destructor.
     */
    virtual ~McsfDcmPrintObject();

    void UpdatePrintJobObject(McsfPrintJobObject* pPrintJobObject);

    const std::vector<std::string>& GetHGfilePathList() const;

    const std::string& GetSPfilePath() const;

    const std::string& GetFileRootDir() const;

    /* load images, presentation states and structured reports */

    /** loads an image which is contained in the database
     *  and creates a default presentation state for the image.
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @param studyUID study instance UID of the image
     *  @param seriesUID series instance UID of the image
     *  @param instanceUID SOP instance UID of the image
     *  @param changeStatus if true the image file is marked 'reviewed' (not new)
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition loadImage(const char *studyUID, const char *seriesUID, const char *instanceUID, OFBool changeStatus = OFFalse);

    /** loads an image (which need not be contained in the database)
     *  and creates a default presentation state for the image.
     *  This method does not acquire a database lock.
     *  @param filename path and filename of the image to be loaded
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition loadImage(const char *filename);

    /** loads an image which referenced by the current presentation
     *  state and needs to be contained in the database.
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @param idx index of the image to be loaded (< getNumberOfImageReferences())
     *  @param changeStatus if true the image file is marked 'reviewed' (not new)
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition loadReferencedImage(size_t idx, OFBool changeStatus = OFFalse);

    /** loads a presentation state which is contained in the database.
     *  The first image referenced in presentation state is also looked up in the
     *  database, loaded, and attached to the presentation state.
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @param studyUID study instance UID of the presentation state
     *  @param seriesUID series instance UID of the presentation state
     *  @param instanceUID SOP instance UID of the presentation state
     *  @param changeStatus if true the pstate and (first) image file is marked 'reviewed' (not new)
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition loadPState(const char *studyUID, const char *seriesUID, const char *instanceUID, OFBool changeStatus = OFFalse);

    /** loads a presentation state and an image (which need not be contained in the database)
     *  and attaches the image to the presentation state (if specified, otherwise the current
     *  image will be used).
     *  This method does not acquire a database lock.
     *  @param pstName path and filename of the presentation state to be loaded
     *  @param imgName path and filename of the image to be loaded
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition loadPState(const char *pstName, const char *imgName = NULL);

    /** saves the current presentation state in the same directory
     *  in which the database index file resides. The filename is generated automatically.
     *  A new SOP Instance UID is assigned whenever a presentation state is saved.
     *  After successfully storing the presentation state, the database index is updated
     *  to include the new object.
     *  This method releases under any circumstances the database lock if it exists.
     *  @param replaceSOPInstanceUID flag indicating whether the
     *    SOP Instance UID should be replaced by a new UID.
     *    If true, a new UID is always generated. If false, a new
     *    UID is generated only if no UID has been assigned before.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition savePState(OFBool replaceSOPInstanceUID);

    /** saves the current presentation state in a file with the given path and filename.
     *  A new SOP Instance UID is assigned whenever a presentation state is saved.
     *  This method does not acquire a database lock and does not register
     *  the saved presentation state in the database.
     *  @param filename path and filename under which the presentation state is to be saved
     *  @param replaceSOPInstanceUID flag indicating whether the
     *    SOP Instance UID should be replaced by a new UID.
     *    If true, a new UID is always generated. If false, a new
     *    UID is generated only if no UID has been assigned before.
     *  @param explicitVR selects the transfer syntax to be written. True (the default) selects
     *    Explicit VR Little Endian, False selects Implicit VR Little Endian.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition savePState(const char *filename, OFBool replaceSOPInstanceUID, OFBool explicitVR=OFTrue);

    /** saves the DICOM image that is currently attached to the presentation state
     *  in a file with the given path and filename.
     *  This method does not acquire a database lock and does not register
     *  the saved presentation state in the database.
     *  @param filename path and filename under which the image is to be saved
     *  @param explicitVR selects the transfer syntax to be written. True (the default) selects
     *    Explicit VR Little Endian, False selects Implicit VR Little Endian.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveCurrentImage(const char *filename, OFBool explicitVR=OFTrue);

    /** returns a reference to the current presentation state.
     *  This reference will become invalid when the DVInterface object is deleted,
     *  a different image or presentation state is loaded
     *  (using loadPState or loadImage) or when resetPresentationState() is called.
     *  @return reference to the current presentation state
     */
    DVPresentationState& getCurrentPState()
    {
      return *pState;
    }

    /** returns a reference to the print handler.
     *  This reference remains valid as long as the DVInterface object exists.
     *  @return reference to the current print handler
     */
    DVPSStoredPrint& getPrintHandler()
    {
      return *pPrint;
    }

    /** resets the presentation state object to the status it had immediately after the
     *  last successful operation of "loadImage" or "loadPState". A state can also explicitly
     *  specified as such a "reset state" by using the method saveCurrentPStateForReset().
     *  Attention: The last reference returned by getCurrentPState() becomes invalid
     *  when this method is called.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition resetPresentationState();

    /** saves the current state of the presentation state object to be used for
     *  resetPresentationState(). This is e.g. useful after registration of additional images
     *  directly after a new images has been loaded.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveCurrentPStateForReset();

    /** returns the review status of the currently selected instance.
     *  May be called only if a valid instance selection exists - see selectInstance().
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @return instance review status
     */
    DVIFhierarchyStatus getInstanceStatus() ;

    /** returns the escription of the currently selected instance.
     *  May be called only if a valid instance selection exists - see selectInstance().
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @return Instance Description or NULL if absent or not selected.
     */
    const char *getInstanceDescription();

    /** returns the Presentation Label of the currently selected instance.
     *  May be called only if a valid instance selection exists - see selectInstance().
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @return Presentation Label or NULL if absent or not selected.
     */
    const char *getPresentationLabel();

    /** creates a dump of the contents of a DICOM file and displays it on-screen.
     *  A separate application or process is launched to handle the dump and display.
     *  This call returns when the dump operation has successfully been launched.
     *  No information about the status or success of the process itself is being made
     *  available.
     *  This method does not acquire a database lock.
     *  @param filename path of file to be displayed.
     *  @return EC_Normal when the process has successfully been launched,
     *     an error condition otherwise.
     */
    OFCondition dumpIOD(const char *filename);

    /** creates a dump of the contents of a DICOM file and displays it on-screen.
     *  A separate application or process is launched to handle the dump and display.
     *  This call returns when the dump operation has successfully been launched.
     *  No information about the status or success of the process itself is being made
     *  available.
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @param studyUID Study Instance UID of the IOD to be dumped. Must be an IOD
     *     contained in the database.
     *  @param seriesUID Series Instance UID of the IOD to be dumped. Must be an IOD
     *     contained in the database.
     *  @param instanceUID SOP Instance UID of the IOD to be dumped. Must be an IOD
     *     contained in the database.
     *  @return EC_Normal when the process has successfully been launched,
     *     an error condition otherwise.
     */
    OFCondition dumpIOD(const char *studyUID, const char *seriesUID, const char *instanceUID);

    /** checks the contents of a DICOM file and displays an evaluation report on the screen.
     *  A separate application or process is launched to handle the evaluation and display.
     *  This call returns when the check operation has successfully been launched.
     *  No information about the status or success of the process itself is being made
     *  available.
     *  This method does not acquire a database lock.
     *  @param filename path of file to be checked.
     *  @return EC_Normal when the process has successfully been launched,
     *     an error condition otherwise.
     */
    OFCondition checkIOD(const char *filename);

    /** checks the contents of a DICOM file and displays an evaluation report on the screen.
     *  A separate application or process is launched to handle the evaluation and display.
     *  This call returns when the check operation has successfully been launched.
     *  No information about the status or success of the process itself is being made
     *  available.
     *  This method acquires a database lock which must be explicitly freed by the user.
     *  @param studyUID Study Instance UID of the IOD to be checked. Must be an IOD
     *     contained in the database.
     *  @param seriesUID Series Instance UID of the IOD to be checked. Must be an IOD
     *     contained in the database.
     *  @param instanceUID SOP Instance UID of the IOD to be checked. Must be an IOD
     *     contained in the database.
     *  @return EC_Normal when the process has successfully been launched,
     *     an error condition otherwise.
     */
    OFCondition checkIOD(const char *studyUID, const char *seriesUID, const char *instanceUID);

    /** saves a monochrome bitmap as a DICOM Secondary Capture image.
     *  The bitmap must use one byte per pixel, left to right, top to bottom
     *  order of the pixels. 0 is interpreted as black, 255 as white.
     *  @param filename the file name or path under which the image is saved.
     *  @param pixelData a pointer to the image data. Must contain at least
     *    width*height bytes of data.
     *  @param width the width of the image, must be <= 0xFFFF
     *  @param height the height of the image, must be <= 0xFFFF
     *  @param aspectRatio the pixel aspect ratio as width/height. If omitted, a pixel
     *    aspect ratio of 1/1 is assumed.
     *  @param explicitVR selects the transfer syntax to be written.
     *    True selects Explicit VR Little Endian, False selects Implicit VR Little Endian.
     *  @param instanceUID optional parameter containing the SOP Instance UID to be written.
     *    This parameter should be omitted unless the SOP Instance UID needs to be controlled
     *    externally.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveDICOMImage(
      const char *filename,
      const void *pixelData,
      unsigned long width,
      unsigned long height,
      double aspectRatio=1.0,
      OFBool explicitVR=OFTrue,
      const char *instanceUID=NULL);

    /** saves a monochrome bitmap as a DICOM Secondary Capture image
     *  in the same directory in which the database index file resides.
     *  The filename is generated automatically.
     *  When the image is stored successfully, the database index is updated
     *  to include the new object.
     *  This method releases under any circumstances the database lock if it exists.
     *  @param pixelData a pointer to the image data. Must contain at least
     *    width*height bytes of data.
     *  @param width the width of the image, must be <= 0xFFFF
     *  @param height the height of the image, must be <= 0xFFFF
     *  @param aspectRatio the pixel aspect ratio as width/height. If omitted, a pixel
     *    aspect ratio of 1/1 is assumed.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveDICOMImage(
      const void *pixelData,
      unsigned long width,
      unsigned long height,
      double aspectRatio=1.0);

    /** saves a monochrome bitmap as a DICOM Hardcopy Grayscale image.
     *  The bitmap must use 16 bits per pixel, left to right, top to bottom
     *  order of the pixels. It is assumed that only values 0..4095 are used.
     *  @param filename the file name or path under which the image is saved.
     *  @param pixelData a pointer to the image data. Must contain at least
     *    width*height*2 bytes of data.
     *  @param width the width of the image, must be <= 0xFFFF
     *  @param height the height of the image, must be <= 0xFFFF
     *  @param aspectRatio the pixel aspect ratio as width/height. If omitted, a pixel
     *    aspect ratio of 1/1 is assumed.
     *  @param explicitVR selects the transfer syntax to be written.
     *    True selects Explicit VR Little Endian, False selects Implicit VR Little Endian.
     *  @param instanceUID optional parameter containing the SOP Instance UID to be written.
     *    This parameter should be omitted unless the SOP Instance UID needs to be controlled
     *    externally.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveHardcopyGrayscaleImage(
      const char *filename,
      const void *pixelData,
      unsigned long width,
      unsigned long height,
      double aspectRatio=1.0,
      OFBool explicitVR=OFTrue,
      const char *instanceUID=NULL);

	OFCondition saveColorImage(
    const char *filename,
    const void *pixelData,
    unsigned long width,
    unsigned long height,
    double aspectRatio=1.0,
    OFBool explicitVR=OFTrue,
    const char *instanceUID=NULL);

    OFCondition HandlePrintingImage(
    const char *instanceUID=NULL);

    OFCondition HandlePrintingImageSetInfo(
    const char *instanceUID=NULL);

    /** saves a monochrome bitmap as a DICOM Hardcopy Grayscale image
     *  in the same directory in which the database index file resides.
     *  The filename is generated automatically.
     *  When the image is stored successfully, the database index is updated
     *  to include the new object.
     *  This method releases under any circumstances the database lock if it exists.
     *  @param pixelData a pointer to the image data. Must contain at least
     *    width*height*2 bytes of data.
     *  @param width the width of the image, must be <= 0xFFFF
     *  @param height the height of the image, must be <= 0xFFFF
     *  @param aspectRatio the pixel aspect ratio as width/height. If omitted, a pixel
     *    aspect ratio of 1/1 is assumed.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveHardcopyGrayscaleImage(
      const void *pixelData,
      unsigned long width,
      unsigned long height,
      double aspectRatio=1.0);

	OFCondition saveColorImage(
	  const void *pixelData,
	  unsigned long width,
	  unsigned long height,
	  double aspectRatio=1.0);

    /** loads a Stored Print object (which need not be contained in the database) into memory.
     *  Attention: The current print job (Stored Print object) will be deleted by doing this.
     *  This method does not acquire a database lock.
     *  @param filename path and filename of the Stored Print object to be loaded
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition loadStoredPrint(const char *filename);

    /** saves the current print job as a Stored Print object.
     *  @param filename the file name or path under which the image is saved.
     *  @param writeRequestedImageSize if false, the Requested Image Size attributes are not written,
     *    e. g. because they are not supported by the target printer.
     *  @param explicitVR selects the transfer syntax to be written.
     *    True selects Explicit VR Little Endian, False selects Implicit VR Little Endian.
     *  @param instanceUID optional parameter containing the SOP Instance UID to be written.
     *    This parameter should be omitted unless the SOP Instance UID needs to be controlled
     *    externally.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveStoredPrint(
      const char *filename,
      OFBool writeRequestedImageSize,
      OFBool explicitVR=OFTrue,
      const char *instanceUID=NULL);

    /** saves the current print job as a Stored Print object
     *  in the same directory in which the database index file resides.
     *  The filename is generated automatically.
     *  When the image is stored successfully, the database index is updated
     *  to include the new object.
     *  This method releases under any circumstances the database lock if it exists.
     *  @param writeRequestedImageSize if false, the Requested Image Size attributes are not written,
     *    e. g. because they are not supported by the target printer.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition saveStoredPrint(OFBool writeRequestedImageSize);

    /** gets the number of Hardcopy Grayscaleimages currently registered by the stored print object.
     *  @return number of images.
     */
    size_t getNumberOfPrintPreviews();

    /** loads a Hardcopy Grayscale image registered by the stored print object and creates a preview.
     *  The preview bitmap is implicitly scaled to fit into the rectangle specified by
     *  setMaxPrintPreviewWidthHeight().
     *  @param idx index of the image, must be < getNumberOfPrintPreviews()
     *  @param printLUT OFTrue if presentation LUT should be interpreted as a print presentation LUT
     *    (default, in this case there is no implicit scaling of the input width of the LUT and,
     *    therefore, the VOI transformation - which is absent for print - is used),
     *    OFFalse otherwise (softcopy interpretation of a presentation LUT)
     *  @param changeStatus if true the hardcopy grayscale image file is marked 'reviewed' (not new)
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition loadPrintPreview(size_t idx, OFBool printLUT = OFTrue, OFBool changeStatus = OFFalse);

    /** removes a currently loaded Hardcopy Grayscale image from memory.
     */
    void unloadPrintPreview();

    /** gets number of bytes used for the print preview bitmap.
     *  (depends on width, height and depth)
     *  @return number of bytes used for the preview bitmap
     */
    unsigned long getPrintPreviewSize();

    /** sets the maximum print preview bitmap width and height.
     *  Larger images are scaled down (according to the pixel aspect ratio) to fit into
     *  the specified rectangle.
     *  Attention: If the values differ from the the previous ones the currently loaded
     *  hardcopy grayscale image (preview) is automatically detroyed and has to be re-loaded.
     *  @param width maximum width of preview bitmap (in pixels)
     *  @param height maximum height of preview bitmap (in pixels)
     */
    void setMaxPrintPreviewWidthHeight(unsigned long width, unsigned long height);

    /** gets width and height of print preview bitmap.
     *  The return values depend on the current maximum preview bitmap width/height values!
     *  @param width upon success, the bitmap width (in pixels) is returned in this parameter
     *  @param height upon success, the bitmap height (in pixels) is returned in this parameter
     *  @return EC_Normal upon success, an error code otherwise
     */
    OFCondition getPrintPreviewWidthHeight(unsigned long &width, unsigned long &height);

    /** writes the bitmap data of the print preview image into the given buffer.
     *  The storage area must be allocated and deleted from the calling method.
     *  @param bitmap pointer to storage area where the pixel data is copied to (array of 8 bit values)
     *  @param size specifies size of the storage area in bytes
     *  @return EC_Normal upon success, an error code otherwise
     */
    OFCondition getPrintPreviewBitmap(void *bitmap, unsigned long size);

    /** returns number of presentation states referencing the currently selected image.
     *  If no instance is currently selected or the selected instance is a presentation
     *  state, returns an error.
     *  @return number of presentation states, 0 if none available or an error occurred
     */
    Uint32 getNumberOfPStates();

    /** selects and loads specified presentation state referencing the currently selected
     *  image.
     *  @param idx index to be selected, must be < getNumberOfPStates()
     *  @param changeStatus if true the presentation state is marked 'reviewed' (not new)
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition selectPState(Uint32 idx, OFBool changeStatus = OFFalse);

    /** returns description of specified presentation state referencing the currently
     *  selected image.
     *  @param idx index to be selected, must be < getNumberOfPStates()
     *  @return presentation state description or NULL idx is invalid
     */
    const char *getPStateDescription(Uint32 idx);

    /** returns label of specified presentation state referencing the currently
     *  selected image.
     *  @param idx index to be selected, must be < getNumberOfPStates()
     *  @return presentation state label or NULL idx is invalid
     */
    const char *getPStateLabel(Uint32 idx);

    /** checks whether display correction is possible (in principle),
     *  i.e. a valid monitor characteristics description exists
     *  and current system is a low-cost system (without built-in
     *  calibration).
     *  @param transform display transform to be checked (default: GSDF)
     *  @return OFTrue if display transform is possible, OFFalse otherwise
     */
    OFBool isDisplayTransformPossible(DVPSDisplayTransform transform = DVPSD_GSDF);

    /** sets ambient light value for the display transformation.
     *  @param value ambient light value to be set
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition setAmbientLightValue(double value);

    /** returns ambient light value for the display transformation.
     *  @param value returned ambient light value
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition getAmbientLightValue(double &value);

    /* print related methods */

    /** selects the current printer. Also adjusts the destination AE title and the
     *  printer name attribute within the Stored Print object.
     *  @param targetID one of the printer target IDs returned by getTargetID().
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition setCurrentPrinter(const char *targetID);

    /** gets the current printer's target ID.
     *  @return printer target ID, can be NULL if no printer is defined
     *   in the configuration file.
     */
    const char *getCurrentPrinter();

    /** sets the (optional) print medium type.
     *  @param value new attribute value, may be NULL.
     *    The caller is responsible for making sure
     *    that the value is valid for the selected printer.
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition setPrinterMediumType(const char *value);

    /** gets the (optional) print medium type.
     *  @return medium type, may be NULL.
     */
    const char *getPrinterMediumType();

    /** sets the (optional) printer film destination.
     *  @param value new attribute value, may be NULL.
     *    The caller is responsible for making sure
     *    that the value is valid for the selected printer.
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition setPrinterFilmDestination(const char *value);

    /** gets the (optional) printer film destination.
     *  @return printer film destination, may be NULL or empty string.
     */
    const char *getPrinterFilmDestination();

    /** sets the (optional) printer film session label.
     *  @param value new attribute value, may be NULL.
     *    The caller is responsible for making sure
     *    that the value is valid for the selected printer.
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition setPrinterFilmSessionLabel(const char *value);

    /** gets the (optional) printer film session label.
     *  @return printer film session label, may be NULL or empty string.
     */
    const char *getPrinterFilmSessionLabel();

    /** sets the (optional) print priority.
     *  @param value new attribute value, may be NULL.
     *    The caller is responsible for making sure
     *    that the value is valid for the selected printer.
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition setPrinterPriority(const char *value);

    /** gets the (optional) print priority.
     *  @return print priority, may be NULL or empty string.
     */
    const char *getPrinterPriority();

    /** sets the (optional) print session owner ID.
     *  @param value new attribute value, may be NULL.
     *    The caller is responsible for making sure
     *    that the value is valid for the selected printer.
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition setPrinterOwnerID(const char *value);

    /** gets the (optional) print session owner ID.
     *  @return print session owner ID, may be NULL or empty string.
     */
    const char *getPrinterOwnerID();

    /** sets the (optional) print number of copies.
     *  @param value new attribute value, may be 0.
     *    The caller is responsible for making sure
     *    that the value is valid for the selected printer.
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition setPrinterNumberOfCopies(unsigned long value);

    /** gets the (optional) print number of copies.
     *  @return print number of copies, 0 if absent.
     */
    unsigned long getPrinterNumberOfCopies();

    /** resets the settings for basic film session (everything that
     *  is not managed by the Stored Print object) to initial state.
     *  Affects medium type, film destination, film session label,
     *  priority, owner ID, and number of copies.
     */
    void clearFilmSessionSettings();

    /** sets the LUT with the given identifier
     *  in the Presentation State as current Presentation LUT.
     *  @param lutID LUT identifier, as returned by getLUTID().
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition selectDisplayPresentationLUT(const char *lutID);

    /** if the Presentation State contains an active
     *  Presentation LUT that was set with selectDisplayPresentationLUT(),
     *  return the corresponding LUT identifier.
     *  @return lutID if found, NULL or empty string otherwise.
     */
    const char *getDisplayPresentationLUTID();

    /** sets the LUT with the given identifier in the Stored Print object
     *  as current Presentation LUT. This LUT overrides the settings made
     *  for the separate image boxes, it can be deactivated using the method
     *  DVPSStoredPrint::setDefaultPresentationLUT().
     *  @param lutID LUT identifier, as returned by getLUTID().
     *  @return EC_Normal if successful, an error code otherwise.
     */
    OFCondition selectPrintPresentationLUT(const char *lutID);

    /** if the Stored Print object contains an active
     *  Presentation LUT that was set with selectPrintPresentationLUT(),
     *  return the corresponding LUT identifier.
     *  @return lutID if found, NULL or empty string otherwise.
     */
    const char *getPrintPresentationLUTID();

    /** Initiates the creation of a DICOM Basic Film Session SOP Instance in the printer.
     *  This method stores all Basic Film Session related attributes that are managed by this object
     *  in a DICOM dataset and passes the result to the embedded Stored Print object which manages
     *  the further communication.
     *  @param printHandler print communication handler, association must be open.
     *  @param plutInSession true if printer expects referenced presentation LUT sequence, illumination
     *    and reflected ambient light in basic film session, false if it expects them in basic film box.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition printSCUcreateBasicFilmSession(DVPSPrintMessageHandler& printHandler, OFBool plutInSession);

    /* annotation interface */

    /** gets the current setting of the annotation activity annotation flag.
     *  @return OFTrue if annotation is on, OFFalse otherwise.
     */
    OFBool isActiveAnnotation() { return activateAnnotation; }

    /** gets the current setting of the Prepend Date/Time annotation flag.
     *  @return OFTrue if Prepend Date/Time is on, OFFalse otherwise.
     */
    OFBool getPrependDateTime() { return prependDateTime; }

    /** gets the current setting of the Prepend Printer Name annotation flag.
     *  @return OFTrue if Prepend Printer Name is on, OFFalse otherwise.
     */
    OFBool getPrependPrinterName() { return prependPrinterName; }

    /** gets the current setting of the Prepend Lighting annotation flag.
     *  @return OFTrue if Prepend Lighting is on, OFFalse otherwise.
     */
    OFBool getPrependLighting() { return prependLighting; }

    /** gets the current annotation text.
     *  @return annotation text, may be NULL or empty string.
     */
    const char *getAnnotationText() { return annotationText.c_str(); }

    /** switches annotation printing on/off
     *  @param value OFTrue if annotation is switched on, OFFalse otherwise.
     */
    void setActiveAnnotation(OFBool value) { activateAnnotation=value; }

    /** sets the Prepend Date/Time annotation flag.
     *  @param value OFTrue if Prepend Date/Time is switched on, OFFalse otherwise.
     */
    void setPrependDateTime(OFBool value) { prependDateTime=value; }

    /** sets the Prepend Printer Name annotation flag.
     *  @param value OFTrue if Prepend Printer Name is switched on, OFFalse otherwise.
     */
    void setPrependPrinterName(OFBool value) { prependPrinterName=value; }

    /** sets the Prepend Lighting annotation flag.
     *  @param value OFTrue if Prepend Lighting is switched on, OFFalse otherwise.
     */
    void setPrependLighting(OFBool value) { prependLighting=value; }

    /** sets the current annotation text.
     *  @param value new text, may be NULL.
     */
    void setAnnotationText(const char *value);

    /** disables internal settings for image and presentation state.
     *  Called when a new SR object is loaded and the current
     *  image/presentation state are hidden consequently.
     */
    void disableImageAndPState();

    /// \brief the HG,SP file storage root path
    std::string m_sFileRootDir;

    std::vector<std::string> m_sHGfilePathList;

    std::string m_sSPfilePath;

private:

    /** private undefined copy constructor
     */
    McsfDcmPrintObject(const McsfDcmPrintObject&);

    /** private undefined assignment operator
     */
    McsfDcmPrintObject& operator=(const McsfDcmPrintObject&);

    OFCondition makeNewStoreFileName(
        const char      *SOPClassUID,
        const char      * /* SOPInstanceUID */ ,
        char            *newImageFileName);

    /** helper function that exchanges the current presentation state and image
     *  by the pointers passed and frees the old ones.
     *  @param newState new presentation state, must not be NULL
     *  @param image image file
     *  @param state presentation state if newState was not created from image.
     *  @return EC_Normal upon success, an error code otherwise.
     */
    OFCondition exchangeImageAndPState(DVPresentationState *newState, DcmFileFormat *image, DcmFileFormat *state=NULL);

    /* member variables */

    /** pointer to the current print handler object
     */
    DVPSStoredPrint *pPrint;

    /** pointer to the current presentation state object
     */
    DVPresentationState *pState;

    /** pointer to the stored presentation state object (if any)
     */
    DVPresentationState *pStoredPState;

    /** pointer to the current DICOM image attached to the presentation state
     */
    DcmFileFormat *pDicomImage;

    /** pointer to the current DICOM dataset containing the loaded presentation state.
     *  Is NULL when the presentation state has been created "on the fly" from image.
     */
    DcmFileFormat *pDicomPState;

    /** pointer to the current hardcopy grayscale image (bitmap information only)
     */
    DicomImage *pHardcopyImage;

    /** list of display function object
     */
    DiDisplayFunction *displayFunction[DVPSD_max];

    /** minimum width of print bitmap (used for implicit scaling)
     */
    unsigned long minimumPrintBitmapWidth;

    /** minimum height of print bitmap (used for implicit scaling)
     */
    unsigned long minimumPrintBitmapHeight;

    /** maximum width of print bitmap (used for implicit scaling)
     */
    unsigned long maximumPrintBitmapWidth;

    /** maximum height of print bitmap (used for implicit scaling)
     */
    unsigned long maximumPrintBitmapHeight;

    /** maximum width of print preview bitmap
     */
    unsigned long maximumPrintPreviewWidth;

    /** maximum height of print preview bitmap
     */
    unsigned long maximumPrintPreviewHeight;

    /** maximum width of (optional) preview image
     */
    unsigned long maximumPreviewImageWidth;

    /** maximum height of (optional) preview image
     */
    unsigned long maximumPreviewImageHeight;

    /** target ID of current printer, empty if no printer exists in config file
     */
    OFString currentPrinter;

     /** config file identifier of LUT currently selected as Display Presentation LUT
     */
    OFString displayCurrentLUTID;

     /** config file identifier of LUT currently selected as Print Presentation LUT
     */
    OFString printCurrentLUTID;

    /** printer medium type identifier, may be empty. VR=CS, VM=1
     */
    OFString printerMediumType;

    /** printer film destination identifier, may be empty. VR=CS, VM=1
     */
    OFString printerFilmDestination;

    /** printer film session label, may be empty. VR=LO, VM=1
     */
    OFString printerFilmSessionLabel;

    /** printer number of copies
     */
    unsigned long printerNumberOfCopies;

    /** printer print priority, may be empty. VR=CS, VM=1,
     *  enumerated values: HIGH, MED, LOW
     */
    OFString printerPriority;

    /** printer film session owner ID, may be empty. VR=SH, VM=1
     */
    OFString printerOwnerID;

    /** true if annotation should be created when spooling print job
     */
    OFBool activateAnnotation;

    /** true if date and time should be prepended to annotation text
     */
    OFBool prependDateTime;

    /** true if printer name should be prepended to annotation text
     */
    OFBool prependPrinterName;

    /** true if reflected ambient light and illumination should be prepended to annotation text
     */
    OFBool prependLighting;

    /** annotation text (if any)
     */
    OFString annotationText;

    McsfPrintJobObject* m_pPrintJobObject;
};

MCSF_FILMING_END_NAMESPACE
#endif
