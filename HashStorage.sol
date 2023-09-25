// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract HashStorage {

    struct ImageData {
        uint256 id;
        string imageHash;
        uint256 timeOfSave;
    }
    mapping(string => ImageData) private imageHashToData;
    ImageData[] public imageDataList;
    string[]  hashData;
    uint256 idCounter;
    function compare(string memory hash) external view returns (bool) {

        uint256 length =  getHashStorageLength();
        uint256 i = 0;
        bool isFound = false;
        while(i <= length) {
            isFound = keccak256(abi.encodePacked((hash))) == keccak256(abi.encodePacked((hashData[i])));
            if(isFound){
                break ;
            }
            i++;
        }
        return(isFound);
    }

    function Store(string memory _hash) external {
        idCounter++;
        hashData.push(_hash);
        StoreHashData(idCounter,_hash);
    }

    function getHashData()external view returns (
        uint256 time,
        string memory hash
    ) {
    
        return(block.timestamp, string(hashData[0]));
    }

    function getHashStorageLength() public  view returns (uint256) {
        return  hashData.length;
    }

    function getHashDataByIndex(uint256 index) external view returns (string memory hash){
        return(string(hashData[index]));
    }

    function StoreHashData(uint256 id,string memory _hash) internal {
        imageDataList.push(ImageData(id,_hash, block.timestamp));
    }
   function getImageDataList() external view returns (ImageData[] memory) {
        return imageDataList;
    }
   function getImageDataLength() external view returns(uint256){
        return imageDataList.length;
   }
   
   function getHashAtIndex(uint256 index) external view returns(ImageData memory){
    return imageDataList[index];
   }
   function checkIfHashExists(string memory hash) external pure returns(ImageData memory){
    
   }


}
